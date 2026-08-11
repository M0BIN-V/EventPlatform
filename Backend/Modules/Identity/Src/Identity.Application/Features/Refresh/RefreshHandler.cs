using Identity.Application.Common.Contracts.ApplicationServices;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Refresh;

public class RefreshHandler(
    IRefreshTokenManager refreshTokenManager,
    IValidator<RefreshRequest> validator,
    UserManager<User> userManager,
    IAccessTokenService accessTokenService,
    IIdentityUnitOfWork uow) :
    Handler<RefreshRequest, RefreshResponse>
{
    public override async Task<RefreshResponse> HandleAsync(
        RefreshRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var result = await refreshTokenManager.RotateAsync(request.RefreshToken, ct);

        await uow.SaveChangesAsync(ct);

        return await result.Match<Task<RefreshResponse>>(
            async successResult =>
            {
                var (rawRefreshToken, refreshToken, user) = successResult;

                var userRoles = await userManager.GetRolesAsync(user);

                var newAccessToken = accessTokenService.GenerateAccessToken(user, userRoles.ToList());

                return new RefreshTokenResponse(newAccessToken, rawRefreshToken);
            },
            tokenAlreadyRotatedError => Task.FromResult<RefreshResponse>(new InvalidRefreshTokenError()),
            userNotFoundError => Task.FromResult<RefreshResponse>(new InvalidRefreshTokenError()),
            invalidRefreshTokenError => Task.FromResult<RefreshResponse>(invalidRefreshTokenError)
        );
    }
}