using Microsoft.EntityFrameworkCore;
using Organizations.Application.Common.Contracts.Persistence;
using Organizations.Domain.Entities;
using Organizations.Infrastructure.Persistence.DbContext;

namespace Organizations.Infrastructure.Persistence.Repositories;

public class OrganizationMemberRepository(EfOrganizationDbContext context) : IOrganizationMemberRepository
{
    public async Task AddAsync(OrganizationMember member, CancellationToken ct = default)
    {
        await context.OrganizationMembers.AddAsync(member, ct);
    }

    public async Task<OrganizationMember?> GetByOrganizationAndUserAsync(Guid organizationId, string userId,
        CancellationToken ct = default)
    {
        return await context.OrganizationMembers
            .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.UserId == userId, ct);
    }
}