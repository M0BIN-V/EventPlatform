using Microsoft.EntityFrameworkCore;
using Files.Domain.Entities;

namespace Files.Infrastructure.Persistence.DbContext;

public class EfFilesDbContext(DbContextOptions<EfFilesDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public const string Schema = "files";
    
    public DbSet<Files.Domain.Entities.File> Files { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfFilesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
