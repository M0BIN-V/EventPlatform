using Files.Application.Common.Contracts.Persistence;
using Files.Application.Common.Contracts.Services;
using Files.Infrastructure.Persistence;
using Files.Infrastructure.Persistence.Repositories;
using Files.Infrastructure.ServiceInstallers;
using Files.Infrastructure.Storage;
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
            .AddScoped<IFilesRepository, FilesRepository>()
            .AddScoped<IFilesUnitOfWork, FilesUnitOfWork>();

        builder.AddS3Storage("event-platform-s3");

        builder.Services.AddScoped<IObjectStorageService, S3ObjectStorageService>();

        return builder;
    }
}