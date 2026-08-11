using BuildingBlocks.Application.Contracts;

namespace Organization.Application.Common.Contracts.Persistence;

public interface IOrganizationRepository
{
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
    Task AddAsync(Domain.Entities.Organization organization, CancellationToken ct = default);
    Task<Domain.Entities.Organization?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Domain.Entities.Organization?> GetBySlugAsync(string slug, CancellationToken ct = default);
}

public interface IOrganizationMemberRepository
{
    Task AddAsync(Domain.Entities.OrganizationMember member, CancellationToken ct = default);
    Task<Domain.Entities.OrganizationMember?> GetByOrganizationAndUserAsync(Guid organizationId, string userId, CancellationToken ct = default);
}

public interface IOrganizationUnitOfWork : IDbContextBase
{
    IOrganizationRepository Organizations { get; }
    IOrganizationMemberRepository Members { get; }
}
