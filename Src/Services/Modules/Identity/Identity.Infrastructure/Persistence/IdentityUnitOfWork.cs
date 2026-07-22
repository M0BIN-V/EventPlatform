using Identity.Application.Common.Contracts.Persistence;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public class IdentityUnitOfWork(IdentityDbContext context) : IIdentityUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}