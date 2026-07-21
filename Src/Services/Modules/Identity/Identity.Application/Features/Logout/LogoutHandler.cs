using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Errors;

namespace Identity.Application.Features.Logout;

public class LogoutHandler(
    IRefreshTokenHasher hasher,
    IValidator<LogoutRequest> validator,
    IRefreshTokenRepository repository,
    IIdentityUnitOfWork uow) :
    Handler<LogoutRequest, LogoutResponse>
{
    public override async Task<LogoutResponse> HandleAsync(
        LogoutRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.Errors;

        var tokenHash = hasher.HashToken(request.RefreshToken);

        var storedRefreshToken = await repository.GetByTokenHashAsync(tokenHash, ct);

        if (storedRefreshToken is null || !storedRefreshToken.Validate(tokenHash))
            return new InvalidRefreshTokenError();

        storedRefreshToken.Revoke();
        
        repository.Update(storedRefreshToken);
        await uow.SaveChangesAsync(ct);

        return new LogoutSuccessResponse("Logged out successfully.");
    }
}