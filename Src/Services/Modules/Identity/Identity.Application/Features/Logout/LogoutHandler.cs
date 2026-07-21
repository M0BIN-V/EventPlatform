using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Common.Contracts.Services;

namespace Identity.Application.Features.Logout;

public class LogoutHandler(
    IValidator<LogoutRequest> validator,
    IRefreshTokenService refreshTokenService) :
    Handler<LogoutRequest, LogoutResponse>
{
    public override async Task<LogoutResponse> HandleAsync(
        LogoutRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.Errors;

        // Validate the refresh token to get the token entity
        var validateResult = await refreshTokenService.ValidateAsync(request.RefreshToken, ct);

        if (validateResult.IsT1)
        {
            // Even if token is invalid, return success (don't expose whether token exists)
            return new LogoutSuccessResponse("Logged out successfully.");
        }

        var (_, token) = validateResult.AsT0;

        // Revoke the refresh token
        await refreshTokenService.RevokeAsync(token, ct);

        return new LogoutSuccessResponse("Logged out successfully.");
    }
}
