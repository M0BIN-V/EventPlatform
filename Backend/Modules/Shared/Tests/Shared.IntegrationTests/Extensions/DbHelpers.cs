using Microsoft.EntityFrameworkCore;

namespace Shared.IntegrationTests.Extensions;

public static class DbHelpers
{
    public static DbContextOptions<TContext> ConfigureOptions<TContext>(string dbConnectionString)
        where TContext : DbContext
    {
        return new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(dbConnectionString)
            .Options;
    }
}