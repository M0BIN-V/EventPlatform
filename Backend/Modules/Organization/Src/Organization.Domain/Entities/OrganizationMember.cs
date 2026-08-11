using BuildingBlocks.Domain.Entities;

namespace Organization.Domain.Entities;

public class OrganizationMember : EntityBase
{
    public required Guid OrganizationId { get; init; }

    public required string UserId { get; init; }

    public required string Role { get; set; }

    public Organization Organization { get; set; } = null!;
}