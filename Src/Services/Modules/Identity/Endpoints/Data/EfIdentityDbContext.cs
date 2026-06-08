using Endpoints.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Endpoints.Data;

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