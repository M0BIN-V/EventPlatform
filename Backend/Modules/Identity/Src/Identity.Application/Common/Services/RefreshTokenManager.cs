using Identity.Application.Common.Contracts.ApplicationServices;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Common.Options;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Common.Services;

public class RefreshTokenManager(
    IRefreshTokenRepository repository,
    IRefreshTokenHasher hasher,
    ISecureTokenGenerator secureTokenGenerator,
    UserManager<User> userManager,
    TimeProvider timeProvider,
    IOptions<RefreshTokenOptions> options) : IRefreshTokenManager
{
    public async Task<RefreshTokenRotationResult> RotateAsync(string rawToken, CancellationToken ct = default)
    {
        var tokenHash = hasher.HashToken(rawToken);

        var storedToken = await repository.GetByTokenHashAsync(tokenHash, ct);

        if (storedToken is null) return new InvalidRefreshTokenError();

        var now = timeProvider.GetUtcNow();

        // Reuse attack detection
        if (storedToken.IsReplaced())
        {
            await RevokeAllUserTokensAsync(storedToken.UserId, RevocationReason.ReuseAttack, now, ct);
            return new TokenAlreadyRotatedError();
        }

        if (!storedToken.IsActive(now)) return new InvalidRefreshTokenError();

        var user = await userManager.FindByIdAsync(storedToken.UserId);

        if (user is null) return new UserNotFoundError(storedToken.UserId);

        var newRawToken = secureTokenGenerator.Generate();

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,

            TokenHash = hasher.HashToken(newRawToken),

            CreatedAt = now,

            ExpiresAt = now.AddDays(options.Value.ExpirationDays)
        };


        storedToken.Rotate(newRefreshToken, now);

        repository.Update(storedToken);
        repository.Add(newRefreshToken);

        return (newRawToken, newRefreshToken, user);
    }

    public async Task<InvalidRefreshTokenError?> RevokeAsync(
        string rawToken,
        RevocationReason reason,
        CancellationToken ct)
    {
        var tokenHash = hasher.HashToken(rawToken);

        var refreshToken = await repository.GetByTokenHashAsync(tokenHash, ct);

        if (refreshToken is null) return new InvalidRefreshTokenError();

        var now = timeProvider.GetUtcNow();

        if (!refreshToken.IsActive(now)) return new InvalidRefreshTokenError();

        refreshToken.Revoke(now, reason);

        repository.Update(refreshToken);

        return null;
    }

    private async Task RevokeAllUserTokensAsync(
        string userId,
        RevocationReason reason,
        DateTimeOffset now,
        CancellationToken ct)
    {
        var tokens = await repository.GetActiveTokensAsync(userId, ct);

        foreach (var token in tokens) token.Revoke(now, reason);

        repository.Update(tokens);
    }
}