using Microsoft.EntityFrameworkCore;

namespace WebApi.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task EnsureMigrationsApplied<TDbContext>()
            where TDbContext : DbContext
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any()) await dbContext.Database.MigrateAsync();
        }
    }
}