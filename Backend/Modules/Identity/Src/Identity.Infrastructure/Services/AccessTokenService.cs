using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application;
using Identity.Application.Common.Contracts.Services;
using Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Services;


public class AccessTokenService(IOptions<JwtOptions> jwtOptions) : IAccessTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateAccessToken(User user, List<string> roles)
    {
        var claims = CreateClaims(user, roles);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private static List<Claim> CreateClaims(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),

            new(ClaimTypes.Email, user.Email ?? string.Empty),

            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
}

public class JwtOptions
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenExpirationMinutes { get; init; } = 15;
}
