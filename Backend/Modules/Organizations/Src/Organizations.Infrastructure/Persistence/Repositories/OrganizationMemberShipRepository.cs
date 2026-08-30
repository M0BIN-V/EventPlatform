using Microsoft.EntityFrameworkCore;
using Organizations.Domain.Entities;

namespace Organizations.Infrastructure.Persistence.Repositories;

public class OrganizationMemberShipRepository(EfOrganizationDbContext context) : IOrganizationMemberRepository
{
    public async Task AddAsync(OrganizationMemberShip memberShip, CancellationToken ct = default)
    {
        await context.OrganizationMemberShips.AddAsync(memberShip, ct);
    }

    public async Task<OrganizationMemberShip?> GetByOrganizationAndUserAsync(Guid organizationId, string userId,
        CancellationToken ct = default)
    {
        return await context.OrganizationMemberShips
            .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId, ct);
    }
}