using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Persistence;

public class EfIdentityDbContext(DbContextOptions<EfIdentityDbContext> options, ILogger<EfIdentityDbContext> logger)
    : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfIdentityDbContext).Assembly);
    }
}