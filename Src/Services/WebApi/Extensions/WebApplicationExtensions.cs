using Microsoft.EntityFrameworkCore;

namespace WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static async Task EnsureMigrationsApplied<TDbContext>(this WebApplication app)
        where TDbContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any()) await dbContext.Database.MigrateAsync();
    }
}