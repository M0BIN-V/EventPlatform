using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    /// <summary>
    ///     Token version used for invalidating all active tokens on security-sensitive changes.
    ///     Increment when: password changes, user disabled, role changes, etc.
    /// </summary>
    public int TokenVersion { get; set; } = 1;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}