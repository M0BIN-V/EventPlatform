using BuildingBlocks.Domain.Entities;

namespace Organization.Domain.Entities;

public class Organization(string name, string slug, string? description, string creatorUserId) : EntityBase
{
    public string Name { get; set; } = name;
    public string Slug { get; set; } = slug;
    public string? Description { get; set; } = description;

    public string CreatorUserId { get; } = creatorUserId;

    public ICollection<OrganizationMember> Members { get; set; } = [];
}