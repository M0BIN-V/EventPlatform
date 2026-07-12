using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public class EfIdentityDbContext(DbContextOptions<EfIdentityDbContext> options)
    : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfIdentityDbContext).Assembly);
    }
}