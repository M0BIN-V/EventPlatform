using System.Security.Cryptography;
using Identity.Application;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Errors;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;

namespace Identity.Infrastructure.Services;

public class RefreshTokenService(
    EfIdentityDbContext dbContext,
    IRefreshTokenHasher hasher,
    IOptions<JwtOptions> jwtOptions,
    UserManager<User> userManager)
    : IRefreshTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<(string RawToken, RefreshToken Entity)> GenerateAsync(
        string userId,
        int expirationDays,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) throw new InvalidOperationException($"User with ID '{userId}' not found.");

        // Generate cryptographically secure token
        var rawToken = GenerateSecureToken();
        var tokenHash = hasher.HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken, ct);
        await dbContext.SaveChangesAsync(ct);

        return (rawToken, refreshToken);
    }

    public async Task<(string RawToken, RefreshToken NewToken)> RotateAsync(
        RefreshToken oldToken,
        int expirationDays,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken ct = default)
    {
        var (rawToken, newToken) = await GenerateAsync(
            oldToken.UserId,
            expirationDays,
            ipAddress,
            userAgent,
            ct);

        // Revoke old token and link it to the new one
        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.ReplacedByTokenId = newToken.Id;

        dbContext.RefreshTokens.Update(oldToken);
        await dbContext.SaveChangesAsync(ct);

        return (rawToken, newToken);
    }

    public async Task RevokeAsync(RefreshToken token, CancellationToken ct = default)
    {
        token.RevokedAt = DateTime.UtcNow;
        dbContext.RefreshTokens.Update(token);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task RevokeAllAsync(string userId, CancellationToken ct = default)
    {
        var activeTokens = await dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in activeTokens) token.RevokedAt = DateTime.UtcNow;

        dbContext.RefreshTokens.UpdateRange(activeTokens);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task CleanupExpiredAsync(CancellationToken ct = default)
    {
        var expiredTokens = await dbContext.RefreshTokens
            .Where(x => x.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(ct);

        if (expiredTokens.Count > 0)
        {
            dbContext.RefreshTokens.RemoveRange(expiredTokens);
            await dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<OneOf<(User User, RefreshToken Token), InvalidRefreshTokenError>> ValidateAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return new InvalidRefreshTokenError("Invalid refresh token.");

        var tokenHash = hasher.HashToken(refreshToken);

        var storedToken = await dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

        if (storedToken is null)
            return new InvalidRefreshTokenError("Invalid refresh token.");

        // Check if token is still valid
        if (storedToken.IsExpired)
            return new InvalidRefreshTokenError("Invalid refresh token.");

        if (storedToken.IsRevoked)
            return new InvalidRefreshTokenError("Invalid refresh token.");
        

        // Check if user is active
        var user = storedToken.User;
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow || !user.EmailConfirmed)
            return new InvalidRefreshTokenError("Invalid refresh token.");

        return (user, storedToken);
    }

    private static string GenerateSecureToken()
    {
        var randomNumber = new byte[64]; // 512 bits of entropy
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}