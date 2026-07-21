namespace Identity.Domain.Entities;

/// <summary>
/// Represents a refresh token used for obtaining new access tokens.
/// </summary>
public class RefreshToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!;

    public User User { get; set; } = null!;
    
    public string TokenHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }
    
    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByTokenId { get; set; }

    public RefreshToken? ReplacedByToken { get; set; }


    public string? IpAddress { get; set; }


    public string? UserAgent { get; set; }


    public bool IsActive => !IsExpired && !IsRevoked;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt.HasValue;
}
