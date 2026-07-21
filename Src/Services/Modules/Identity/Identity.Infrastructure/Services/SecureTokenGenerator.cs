using System.Security.Cryptography;
using Identity.Application.Common.Contracts.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace Identity.Infrastructure.Services;

public class SecureTokenGenerator : ISecureTokenGenerator
{
    public string Generate()
    {
        return WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
    }
}