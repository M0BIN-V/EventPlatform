using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.DbContext;

public class EfIdentityDbContext(DbContextOptions<EfIdentityDbContext> options)
    : IdentityDbContext<User>(options)
{
    public const string Schema = "identity";

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfIdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}