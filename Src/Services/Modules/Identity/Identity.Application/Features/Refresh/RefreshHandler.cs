using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Common.Options;
using Identity.Application.Errors;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.Refresh;

public class RefreshHandler(
    IValidator<RefreshRequest> validator,
    UserManager<User> userManager,
    IOptions<RefreshTokenOptions> options,
    ISecureTokenGenerator secureTokenGenerator,
    IAccessTokenService accessTokenService,
    IRefreshTokenRepository repository,
    IIdentityUnitOfWork uow,
    IRefreshTokenHasher hasher) :
    Handler<RefreshRequest, RefreshResponse>
{
    public override async Task<RefreshResponse> HandleAsync(
        RefreshRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var tokenHash = hasher.HashToken(request.RefreshToken);

        var storedRefreshToken = await repository.GetByTokenHashAsync(tokenHash, ct);

        if (storedRefreshToken is null) return new InvalidRefreshTokenError();

        // handle the case where the refresh token has been rotated to protect against token reuse attacks
        if (storedRefreshToken.IsRotated)
        {
            var activeTokens = await repository.GetActiveTokensAsync(storedRefreshToken.UserId, ct);

            activeTokens.ForEach(r => r.Revoke());

            repository.Update(activeTokens);

            await uow.SaveChangesAsync(ct);

            return new InvalidRefreshTokenError();
        }

        if (!storedRefreshToken.Validate(tokenHash))
            return new InvalidRefreshTokenError();

        var user = await userManager.FindByIdAsync(storedRefreshToken.UserId);
        if (user is null) return new InvalidRefreshTokenError();

        var rawRefreshToken = secureTokenGenerator.Generate();
        var userRoles = await userManager.GetRolesAsync(user);

        var newRefreshToken = new RefreshToken
        {
            TokenHash = hasher.HashToken(rawRefreshToken),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(options.Value.ExpirationDays)
        };

        storedRefreshToken.Rotate(newRefreshToken);

        repository.Add(newRefreshToken);
        repository.Update(storedRefreshToken);

        await uow.SaveChangesAsync(ct);

        var newAccessToken = accessTokenService.GenerateAccessToken(user, userRoles.ToList());

        return new RefreshTokenResponse(newAccessToken, rawRefreshToken);
    }
}