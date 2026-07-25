using Identity.Application.Common.Contracts.ApplicationServices;
using Identity.Application.Common.Contracts.Persistence;

namespace Identity.Application.Features.Logout;

public class LogoutHandler(
    IRefreshTokenManager refreshTokenManager,
    IValidator<LogoutRequest> validator,
    IIdentityUnitOfWork uow) :
    Handler<LogoutRequest, LogoutResponse>
{
    public override async Task<LogoutResponse> HandleAsync(LogoutRequest request, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid) return validationResult.Errors;

        var error = await refreshTokenManager.RevokeAsync(request.RefreshToken, RevocationReason.Logout, ct);

        if (error is not null) return new InvalidRefreshTokenError();

        await uow.SaveChangesAsync(ct);

        return new LogoutSuccessResponse("Logged out successfully.");
    }
}