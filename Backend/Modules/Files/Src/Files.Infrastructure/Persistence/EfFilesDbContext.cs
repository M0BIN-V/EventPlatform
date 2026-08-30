using Microsoft.EntityFrameworkCore;

namespace Files.Infrastructure.Persistence;

public class EfFilesDbContext(DbContextOptions<EfFilesDbContext> options) : DbContext(options)
{
    public const string Schema = "files";

    public DbSet<File> Files { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfFilesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}