using Files.Infrastructure.Persistence.DbContext;
using Files.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Files.Infrastructure;

public static class InfrastructureServiceInstaller
{
    public static IHostApplicationBuilder AddFilesModuleInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<EfFilesDbContext>(
            "event-platform-db",
            null,
            dbContextOpt => dbContextOpt.UseNpgsql(npgOpt => npgOpt
                .MigrationsHistoryTable("__EFMigrationsHistory", EfFilesDbContext.Schema)));

        builder.Services
            .AddScoped<FilesRepository>();

        return builder;
    }
}