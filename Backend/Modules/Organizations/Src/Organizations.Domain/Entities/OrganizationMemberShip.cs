using BuildingBlocks.Domain.Entities;

namespace Organizations.Domain.Entities;

public class OrganizationMemberShip : EntityBase
{
    public required Guid OrganizationId { get; init; }

    public required string UserId { get; init; }

    public required string Role { get; set; }

    public Organization Organization { get; set; } = null!;
}