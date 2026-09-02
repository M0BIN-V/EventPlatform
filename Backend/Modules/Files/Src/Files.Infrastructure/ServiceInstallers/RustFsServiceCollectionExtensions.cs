using Amazon.S3;
using Files.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Files.Infrastructure.ServiceInstallers;

public static class RustFsServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddS3Storage(this IHostApplicationBuilder builder,
        string connectionStringName)
    {
        var configuration = builder.Configuration;

        var connectionString = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' was not found.");

        var options = S3ConnectionOptions.Parse(connectionString);

        var s3Config = new AmazonS3Config
        {
            ServiceURL = options.Endpoint,
            ForcePathStyle = true
        };

        var s3Client = new AmazonS3Client(
            options.AccessKey,
            options.SecretKey,
            s3Config);

        builder.Services.AddSingleton<IAmazonS3>(s3Client);

        return builder;
    }
}