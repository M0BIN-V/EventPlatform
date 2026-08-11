using Identity.Application.Common.Contracts.Persistence;
using Identity.Infrastructure.Persistence.DbContext;

namespace Identity.Infrastructure.Persistence;

public class IdentityUnitOfWork(EfIdentityDbContext context) : IIdentityUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}