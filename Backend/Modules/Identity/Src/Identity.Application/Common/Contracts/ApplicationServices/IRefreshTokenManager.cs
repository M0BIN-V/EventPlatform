namespace Identity.Application.Common.Contracts.ApplicationServices;

public record  RefreshTokenRotationSuccessResult(string RawToken, RefreshToken RefreshToken, User User);

public union RefreshTokenRotationResult(
    RefreshTokenRotationSuccessResult,
    TokenAlreadyRotatedError,
    UserNotFoundError,
    InvalidRefreshTokenError);

public interface IRefreshTokenManager
{
    public Task<RefreshTokenRotationResult> RotateAsync(string rawToken, CancellationToken ct = default);

    public Task<InvalidRefreshTokenError?> RevokeAsync(
        string rawToken,
        RevocationReason reason,
        CancellationToken ct);
}