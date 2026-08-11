using BuildingBlocks.Domain.Entities;

namespace Organization.Domain.Entities;

public class OrganizationMember : EntityBase
{
    public Guid OrganizationId { get; set; }

    public string UserId { get; set; } = null!;

    public string Role { get; set; } = null!;

    public Organization Organization { get; set; } = null!;
}