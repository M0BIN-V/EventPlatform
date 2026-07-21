using System.Security.Cryptography;
using Identity.Application.Common.Contracts.Services;

namespace Identity.Infrastructure.Services;

/// <summary>
/// Service for securely hashing and verifying refresh tokens.
/// Uses PBKDF2 algorithm for password-based key derivation.
/// </summary>
public class RefreshTokenHasher : IRefreshTokenHasher
{
    private const int Iterations = 10000;
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits

    public string HashToken(string token)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Use static method instead of constructor if available
        #pragma warning disable SYSLIB0060
        using var pbkdf2 = new Rfc2898DeriveBytes(token, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashSize);
        #pragma warning restore SYSLIB0060

        // Combine salt and hash: [salt(16) + hash(32)] = 48 bytes
        var hashWithSalt = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, hashWithSalt, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, hashWithSalt, SaltSize, HashSize);

        return Convert.ToBase64String(hashWithSalt);
    }

    public bool VerifyToken(string token, string hash)
    {
        try
        {
            var hashWithSalt = Convert.FromBase64String(hash);

            // Extract salt and hash
            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashWithSalt, 0, salt, 0, SaltSize);

            var storedHash = new byte[HashSize];
            Buffer.BlockCopy(hashWithSalt, SaltSize, storedHash, 0, HashSize);

            // Derive hash from token with extracted salt
            #pragma warning disable SYSLIB0060
            using var pbkdf2 = new Rfc2898DeriveBytes(token, salt, Iterations, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(HashSize);
            #pragma warning restore SYSLIB0060

            // Constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }
}
