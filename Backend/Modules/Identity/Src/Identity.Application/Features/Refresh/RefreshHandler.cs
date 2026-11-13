using Identity.Application.Common.Contracts.ApplicationServices;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Refresh;

public record RefreshTokenResponse(string AccessToken, string RefreshToken);

public union RefreshResponse(RefreshTokenResponse, List<ValidationFailure>, InvalidRefreshTokenError);

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

        var rotationResult = await refreshTokenManager.RotateAsync(request.RefreshToken, ct);

        await uow.SaveChangesAsync(ct);

        return rotationResult switch
        {
            RefreshTokenRotationSuccessResult refreshTokenRotationSuccessResult => await HandleSuccess(
                refreshTokenRotationSuccessResult),
            TokenAlreadyRotatedError => new InvalidRefreshTokenError(),
            UserNotFoundError => new InvalidRefreshTokenError(),
            InvalidRefreshTokenError => new InvalidRefreshTokenError(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<RefreshResponse> HandleSuccess(RefreshTokenRotationSuccessResult successResult)
    {
        var userRoles = await userManager.GetRolesAsync(successResult.User);
        var newAccessToken = accessTokenService.GenerateAccessToken(successResult.User, userRoles.ToList());
        return new RefreshTokenResponse(newAccessToken, successResult.RawToken);
    }
}