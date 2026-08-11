using BuildingBlocks.Domain.Entities;

namespace Organization.Domain.Entities;

public class Organization : EntityBase
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    
    /// <summary>
    /// External reference to Identity UserId who created this organization
    /// </summary>
    public string CreatorUserId { get; set; } = null!;

    public ICollection<OrganizationMember> Members { get; set; } = [];
}
