using Organization.Domain.Entities;

namespace Organization.Application.Common.Contracts.Persistence;

public interface IOrganizationMemberRepository
{
    Task AddAsync(OrganizationMember member, CancellationToken ct = default);

    Task<OrganizationMember?> GetByOrganizationAndUserAsync(Guid organizationId, string userId,
        CancellationToken ct = default);
}