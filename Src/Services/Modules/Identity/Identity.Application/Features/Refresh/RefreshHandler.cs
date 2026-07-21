using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Common.Contracts.Services;

namespace Identity.Application.Features.Refresh;

public class RefreshHandler(
    IValidator<RefreshRequest> validator,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService) :
    Handler<RefreshRequest, RefreshResponse>
{
    public override async Task<RefreshResponse> HandleAsync(
        RefreshRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        // Validate the refresh token
        var validateResult = await refreshTokenService.ValidateAsync(request.RefreshToken, ct);

        if (validateResult.IsT1)
            return validateResult.AsT1;

        var (user, oldToken) = validateResult.AsT0;

        // Rotate the refresh token
        var (newRefreshToken, _) = await refreshTokenService.RotateAsync(
            oldToken,
            7, // This should be configurable
            null,
            null,
            ct);

        // Generate new access token
        var newAccessToken = await tokenService.GenerateAccessTokenAsync(user);

        return new RefreshTokenResponse(newAccessToken, newRefreshToken);
    }
}