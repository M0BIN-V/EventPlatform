using Organization.Infrastructure.Persistence.Repositories;

namespace Organization.Infrastructure.Persistence;

public class OrganizationUnitOfWork(EfOrganizationDbContext context) : IOrganizationUnitOfWork
{
    private IOrganizationMemberRepository? _memberRepository;
    private IOrganizationRepository? _organizationRepository;

    public IOrganizationRepository Organizations =>
        _organizationRepository ??= new OrganizationRepository(context);

    public IOrganizationMemberRepository Members =>
        _memberRepository ??= new OrganizationMemberRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await context.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
    }
}