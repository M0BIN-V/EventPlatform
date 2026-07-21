using System.Security.Cryptography;
using System.Text;
using Identity.Application.Common.Contracts.Services;

namespace Identity.Infrastructure.Services;

public class RefreshTokenHasher : IRefreshTokenHasher
{
    public string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}