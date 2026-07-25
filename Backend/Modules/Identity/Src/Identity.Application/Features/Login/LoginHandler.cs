using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Common.Errors;
using Identity.Application.Common.Options;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.Login;

public class LoginHandler(
    TimeProvider timeProvider,
    IValidator<LoginRequest> validator,
    ISecureTokenGenerator tokenGenerator,
    IRefreshTokenHasher hasher,
    IRefreshTokenRepository repository,
    IIdentityUnitOfWork uow,
    IOptions<RefreshTokenOptions> options,
    UserManager<User> userManager,
    IAccessTokenService accessTokenService) :
    Handler<LoginRequest, LoginResponse>
{
    public override async Task<LoginResponse> HandleAsync(LoginRequest request, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.Errors;

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return new UserNotFoundError(request.Email);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid) return new InvalidPasswordError();

        var userRoles = await userManager.GetRolesAsync(user);

        var accessToken = accessTokenService.GenerateAccessToken(user, userRoles.ToList());

        var rawRefreshToken = tokenGenerator.Generate();

        var now = timeProvider.GetUtcNow();

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = hasher.HashToken(rawRefreshToken),
            CreatedAt = now,
            UserId = user.Id,
            ExpiresAt = now.AddDays(options.Value.ExpirationDays)
        };

        repository.Add(refreshTokenEntity);
        await uow.SaveChangesAsync(ct);

        return new LoginTokenResponse(accessToken, rawRefreshToken);
    }
}