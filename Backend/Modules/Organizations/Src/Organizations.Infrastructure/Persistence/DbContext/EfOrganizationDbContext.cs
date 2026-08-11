using Microsoft.EntityFrameworkCore;
using Organizations.Domain.Entities;

namespace Organizations.Infrastructure.Persistence.DbContext;

public class EfOrganizationDbContext(DbContextOptions<EfOrganizationDbContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public const string Schema = "organization";

    public DbSet<Organizations.Domain.Entities.Organization> Organizations { get; init; } = null!;
    public DbSet<OrganizationMember> OrganizationMembers { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfOrganizationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}