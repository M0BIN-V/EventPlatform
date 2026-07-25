using Identity.Application.Common.Contracts.Persistence;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(
    EfIdentityDbContext context,
    TimeProvider timeProvider) : IRefreshTokenRepository
{
    public void Add(RefreshToken refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
    }

    public async Task<List<RefreshToken>> GetActiveTokensAsync(string userId, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .Where(t => 
                t.UserId == userId &&
                t.RevokedAt == null &&
                t.ReplacedByTokenId != null &&
                t.ExpiresAt!= timeProvider.GetUtcNow())
            .ToListAsync(ct);
    }

    public void Update(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
    }

    public void Update(List<RefreshToken> refreshTokens)
    {
        context.RefreshTokens.UpdateRange(refreshTokens);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, ct);
    }
}