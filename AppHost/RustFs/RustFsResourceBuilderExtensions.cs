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
            $"{name}-access-key",
            accessKey ?? "this-is-access-key",
            secret: true);

        var secretKeyParameter = builder.AddParameter(
            $"{name}-secret-key",
            secretKey ?? "this-is-secret-key",
            secret: true);

        var resource = new RustFsResource(
            name,
            accessKeyParameter.Resource,
            secretKeyParameter.Resource);

        return builder
            .AddResource(resource)
            .WithImage(Image)
            .WithImageTag(ImageTag)
            .WithHttpEndpoint(
                targetPort: 9000,
                name: RustFsResource.S3EndpointName)
            .WithHttpEndpoint(
                targetPort: 9001,
                name: RustFsResource.ConsoleEndpointName)
            .WithEnvironment(
                "RUSTFS_ACCESS_KEY",
                accessKeyParameter)
            .WithEnvironment(
                "RUSTFS_SECRET_KEY",
                secretKeyParameter)
            .WithEnvironment(
                "RUSTFS_ADDRESS",
                "0.0.0.0:9000")
            .WithEnvironment(
                "RUSTFS_CONSOLE_ADDRESS",
                "0.0.0.0:9001")
            .WithEnvironment(
                "RUSTFS_CONSOLE_ENABLE",
                "true")
            .WithEnvironment(
                "RUSTFS_VOLUMES",
                "/data")
            .WithVolume(
                target: "/data",
                name: $"{name}-data")
            .WithHttpHealthCheck(
                "/health",
                endpointName: RustFsResource.S3EndpointName)
            .WithUrlForEndpoint(
                RustFsResource.ConsoleEndpointName,
                url => url.DisplayText = "RustFS Console");
    }

    [AspireExport]
    public static IResourceBuilder<RustFsResource> WithS3Port(
        this IResourceBuilder<RustFsResource> builder,
        int port)
    {
        builder.WithHttpEndpoint(
            port,
            9000,
            RustFsResource.S3EndpointName);

        return builder;
    }

    [AspireExport]
    public static IResourceBuilder<RustFsResource> WithConsolePort(
        this IResourceBuilder<RustFsResource> builder,
        int port)
    {
        builder.WithHttpEndpoint(
            port,
            9001,
            RustFsResource.ConsoleEndpointName);

        return builder;
    }

    [AspireExport]
    public static IResourceBuilder<RustFsResource> WithDataVolume(
        this IResourceBuilder<RustFsResource> builder,
        string? volumeName = null)
    {
        builder.WithVolume(
            target: "/data",
            name: volumeName ?? $"{builder.Resource.Name}-data");

        return builder;
    }
}