using Organization.Application.Common.Contracts.Persistence;
using Organization.Infrastructure.Persistence.DbContext;
using Organization.Infrastructure.Persistence.Repositories;

namespace Organization.Infrastructure.Persistence;

public class OrganizationUnitOfWork(EfOrganizationDbContext context) : IOrganizationUnitOfWork
{
    private IOrganizationRepository? _organizationRepository;
    private IOrganizationMemberRepository? _memberRepository;

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
