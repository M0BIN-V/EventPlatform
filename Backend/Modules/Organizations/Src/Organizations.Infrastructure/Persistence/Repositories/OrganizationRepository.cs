using Microsoft.EntityFrameworkCore;
using Organizations.Domain.Entities;

namespace Organizations.Infrastructure.Persistence.Repositories;

public class OrganizationRepository(EfOrganizationDbContext context) : IOrganizationRepository
{
    public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
    {
        return await context.Organizations
            .AnyAsync(x => x.Slug == slug.ToLowerInvariant(), ct);
    }

    public async Task AddAsync(Organization organization, CancellationToken ct = default)
    {
        await context.Organizations.AddAsync(organization, ct);
    }

    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Organizations
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Organization?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await context.Organizations
            .FirstOrDefaultAsync(x => x.Slug == slug.ToLowerInvariant(), ct);
    }
}