using Microsoft.EntityFrameworkCore;

namespace Organization.Infrastructure.Persistence.Repositories;

public class OrganizationRepository(EfOrganizationDbContext context) : IOrganizationRepository
{
    public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
    {
        return await context.Organizations
            .AnyAsync(x => x.Slug == slug.ToLowerInvariant(), ct);
    }

    public async Task AddAsync(Domain.Entities.Organization organization, CancellationToken ct = default)
    {
        await context.Organizations.AddAsync(organization, ct);
    }

    public async Task<Domain.Entities.Organization?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Organizations
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Domain.Entities.Organization?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await context.Organizations
            .FirstOrDefaultAsync(x => x.Slug == slug.ToLowerInvariant(), ct);
    }
}