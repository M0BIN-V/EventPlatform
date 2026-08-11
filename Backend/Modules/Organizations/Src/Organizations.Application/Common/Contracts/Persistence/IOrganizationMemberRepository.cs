using System.Threading;
using System.Threading.Tasks;
using Organizations.Domain.Entities;

namespace Organizations.Application.Common.Contracts.Persistence;

public interface IOrganizationMemberRepository
{
    Task AddAsync(OrganizationMember member, CancellationToken ct = default);

    Task<OrganizationMember?> GetByOrganizationAndUserAsync(Guid organizationId, string userId,
        CancellationToken ct = default);
}