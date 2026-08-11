using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Organization.Infrastructure.Persistence.DbContext;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EfOrganizationDbContext>
{
    public EfOrganizationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EfOrganizationDbContext>();
        
        var connectionString = "Server=localhost;Port=5432;Database=event-platform-db;User Id=postgres;Password=password;";
        optionsBuilder.UseNpgsql(connectionString, npgOpt => npgOpt
            .MigrationsHistoryTable("__EFMigrationsHistory", EfOrganizationDbContext.Schema));

        return new EfOrganizationDbContext(optionsBuilder.Options);
    }
}
