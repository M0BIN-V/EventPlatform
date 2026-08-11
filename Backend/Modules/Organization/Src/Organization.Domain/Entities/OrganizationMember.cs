using BuildingBlocks.Domain.Entities;

namespace Organization.Domain.Entities;

public class OrganizationMember : EntityBase
{
    public Guid OrganizationId { get; set; }
    
    /// <summary>
    /// External reference to Identity UserId
    /// </summary>
    public string UserId { get; set; } = null!;
    
    /// <summary>
    /// Role within the organization (Owner, Admin, Member)
    /// This is NOT an Identity role, but an Organization-specific role
    /// </summary>
    public string Role { get; set; } = null!;

    public Organization Organization { get; set; } = null!;
}
