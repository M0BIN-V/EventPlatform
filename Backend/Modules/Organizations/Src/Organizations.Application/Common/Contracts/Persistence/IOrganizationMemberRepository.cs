using System.Threading;
using System.Threading.Tasks;
using Organizations.Domain.Entities;

namespace Organizations.Application.Common.Contracts.Persistence;

public interface IOrganizationMemberRepository
{
    Task AddAsync(OrganizationMemberShip memberShip, CancellationToken ct = default);

    Task<OrganizationMemberShip?> GetByOrganizationAndUserAsync(Guid organizationId, string userId,
        CancellationToken ct = default);
}