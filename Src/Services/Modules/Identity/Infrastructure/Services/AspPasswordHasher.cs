using Application.Contracts.Services;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AspPasswordHasher : IPasswordHasher
{
    readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public bool Verify(string password, string hash)
    {
        return _hasher.VerifyHashedPassword(
            null!,
            hash,
            password) != PasswordVerificationResult.Failed;
    }
}