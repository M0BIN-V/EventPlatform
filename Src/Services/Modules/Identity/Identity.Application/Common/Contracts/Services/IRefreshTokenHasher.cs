namespace Identity.Application.Common.Contracts.Services;

/// <summary>
/// Service for hashing and verifying refresh tokens securely.
/// </summary>
public interface IRefreshTokenHasher
{
    /// <summary>
    /// Generates a hash of the refresh token for secure storage.
    /// </summary>
    string HashToken(string token);

    /// <summary>
    /// Verifies a token against its stored hash.
    /// </summary>
    bool VerifyToken(string token, string hash);
}
