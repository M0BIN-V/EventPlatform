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

        // Configure storage options and register S3 adapter as the IObjectStorageService
        builder.Services.Configure<Files.Infrastructure.Storage.FilesStorageOptions>(builder.Configuration.GetSection("FilesStorage"));

        // Register AWS S3 client using options (deferred creation)
        builder.Services.AddSingleton<Amazon.S3.IAmazonS3>(sp =>
        {
            var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<Files.Infrastructure.Storage.FilesStorageOptions>>().Value;
            var config = new Amazon.S3.AmazonS3Config { ServiceURL = opts.Endpoint, ForcePathStyle = opts.UsePathStyle };

            if (!string.IsNullOrWhiteSpace(opts.Region)) config.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(opts.Region);

            return new Amazon.S3.AmazonS3Client(opts.AccessKey, opts.SecretKey, config);
        });

        builder.Services.AddScoped<Files.Application.Common.Contracts.Services.IObjectStorageService, Files.Infrastructure.Storage.S3ObjectStorageService>();

        return builder;
    }
}