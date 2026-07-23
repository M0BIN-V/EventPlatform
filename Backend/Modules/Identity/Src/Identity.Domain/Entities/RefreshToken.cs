using BuildingBlocks.Domain.Entities;

namespace Identity.Domain.Entities;

public class RefreshToken : EntityBase
{
    public required string UserId { get; init; } = null!;

    public User User { get; init; } = null!;

    public required string TokenHash { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public required DateTime ExpiresAt { get; init; }

    public DateTime? RevokedAt { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }
    public RefreshToken? ReplacedByToken { get; private set; }
    public bool IsRotated => ReplacedByToken is not null;

    public string? IpAddress { get; init; }

    public string? UserAgent { get; init; }


    public bool IsActive => !IsExpired && !IsRevoked;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt.HasValue;

    public bool Validate(string refreshTokenHash)
    {
        return refreshTokenHash == TokenHash && IsActive;
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }

    public void Rotate(RefreshToken newRefreshToken)
    {
        if (IsRotated)
            throw new InvalidOperationException("Token has already been rotated.");

        ReplacedByTokenId = newRefreshToken.Id;
        ReplacedByToken = newRefreshToken;
        Revoke();
    }
}