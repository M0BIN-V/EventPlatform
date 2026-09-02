namespace AppHost.RustFs;

public static class RustFsResourceBuilderExtensions
{
    private const string Image = "rustfs/rustfs";
    private const string ImageTag = "latest";

    [AspireExport]
    public static IResourceBuilder<RustFsResource> AddRustFs(
        this IDistributedApplicationBuilder builder,
        string name, string? accessKey = null, string? secretKey = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var accessKeyParameter = builder.AddParameter(
            $"{name}-access-key", accessKey ?? "this-is-access-key", secret: true);

        var secretKeyParameter = builder.AddParameter(
            $"{name}-secret-key", secretKey ?? "this-is-secret-key", secret: true);

        var resource = new RustFsResource(name, accessKeyParameter.Resource, secretKeyParameter.Resource);

        return builder
            .AddResource(resource)
            .WithImage(Image)
            .WithImageTag(ImageTag)
            .WithHttpEndpoint(targetPort: 9000, name: RustFsResource.S3EndpointName)
            .WithHttpEndpoint(targetPort: 9001, name: RustFsResource.ConsoleEndpointName)
            .WithEnvironment("RUSTFS_ACCESS_KEY", accessKeyParameter)
            .WithEnvironment("RUSTFS_SECRET_KEY", secretKeyParameter)
            .WithEnvironment("RUSTFS_ADDRESS", "0.0.0.0:9000")
            .WithEnvironment("RUSTFS_CONSOLE_ADDRESS", "0.0.0.0:9001")
            .WithEnvironment("RUSTFS_CONSOLE_ENABLE", "true")
            .WithEnvironment("RUSTFS_VOLUMES", "/data")
            .WithHttpHealthCheck("/health", endpointName: RustFsResource.S3EndpointName)
            .WithUrlForEndpoint(RustFsResource.ConsoleEndpointName, url => url.DisplayText = "RustFS Console");
    }

    extension(IResourceBuilder<RustFsResource> builder)
    {
        public IResourceBuilder<RustFsResource> EnableNotify()
        {
            builder.WithEnvironment(" RUSTFS_NOTIFY_WEBHOOK_ENABLE_PRIMARY", "on");
            return builder.WithEnvironment("RUSTFS_NOTIFY_ENABLE", "true");
        }

        public IResourceBuilder<RustFsResource> EnableNotifyWebhook(Action<WebHookConfig>? configure = null)
        {
            var config = new WebHookConfig(builder);

            configure?.Invoke(config);

            return builder;
        }

        public IResourceBuilder<RustFsResource> WithWebHook(int port)
        {
            return builder;
        }

        [AspireExport]
        public IResourceBuilder<RustFsResource> WithS3Port(int port)
        {
            return builder.WithHttpEndpoint(port, 9000, RustFsResource.S3EndpointName);
        }

        [AspireExport]
        public IResourceBuilder<RustFsResource> WithConsolePort(int port)
        {
            return builder.WithHttpEndpoint(port, 9001, RustFsResource.ConsoleEndpointName);
        }

        [AspireExport]
        public IResourceBuilder<RustFsResource> WithDataVolume(string? volumeName = null)
        {
            return builder.WithVolume(target: "/data", name: volumeName ?? $"{builder.Resource.Name}-data");
        }
    }
}