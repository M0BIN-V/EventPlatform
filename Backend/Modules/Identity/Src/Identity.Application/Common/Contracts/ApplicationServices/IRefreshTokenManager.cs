namespace Identity.Application.Common.Contracts.ApplicationServices;

[GenerateOneOf]
public partial class RefreshTokenRotationResult : OneOfBase<
    (string, RefreshToken, User),
    TokenAlreadyRotatedError,
    UserNotFoundError,
    InvalidRefreshTokenError>;

public interface IRefreshTokenManager
{
    public Task<RefreshTokenRotationResult> RotateAsync(string rawToken, CancellationToken ct = default);

    public Task<InvalidRefreshTokenError?> RevokeAsync(
        string rawToken,
        RevocationReason reason,
        CancellationToken ct);
}