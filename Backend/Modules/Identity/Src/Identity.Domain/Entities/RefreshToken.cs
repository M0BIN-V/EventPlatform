using BuildingBlocks.Domain.Entities;
using Identity.Domain.Constants;

namespace Identity.Domain.Entities;

public class RefreshToken : EntityBase
{
    public required string UserId { get; init; } = null!;

    public User User { get; init; } = null!;

    public required string TokenHash { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }

    public DateTimeOffset? RevokedAt { get; private set; }
    public RevocationReason RevocationReason { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }
    public RefreshToken? ReplacedByToken { get; private set; }
    public DateTimeOffset? ReplacedAt { get; private set; }

    public string? IpAddress { get; init; }

    public string? UserAgent { get; init; }

    public bool IsExpired(DateTimeOffset now)
    {
        return ExpiresAt <= now;
    }

    public bool IsRevoked()
    {
        return RevokedAt is not null;
    }

    public bool IsReplaced()
    {
        return ReplacedByTokenId is not null;
    }

    public bool IsActive(DateTimeOffset now)
    {
        return
            !IsRevoked() &&
            !IsExpired(now) &&
            !IsReplaced();
    }

    public void Revoke(DateTimeOffset revokedAt, RevocationReason reason)
    {
        if (IsRevoked()) throw new InvalidOperationException("Token already revoked.");

        RevokedAt = revokedAt;
        RevocationReason = reason;
    }

    public void Rotate(RefreshToken newRefreshToken, DateTimeOffset now)
    {
        if (!IsActive(now)) throw new InvalidOperationException("Token is not active.");

        ReplacedByTokenId = newRefreshToken.Id;
        ReplacedByToken = newRefreshToken;
        ReplacedAt = now;

        Revoke(now, RevocationReason.Rotated);
    }
}