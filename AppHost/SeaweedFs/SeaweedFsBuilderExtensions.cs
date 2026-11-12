using System.Text.Json;

// Extensions live in Aspire.Hosting so they're discoverable automatically.
namespace AppHost.SeaweedFs;

public static class SeaweedFsBuilderExtensions
{
    private const string DataPath = "/data";

    /// <summary>
    /// Adds a SeaweedFS container (master + volume + filer + S3 gateway) to the app model.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">Resource name.</param>
    /// <param name="s3Port">Optional fixed host port for the S3 API (container port 8333).</param>
    /// <param name="masterPort">Optional fixed host port for the master API (container port 9333).</param>
    /// <param name="accessKey">Optional access-key parameter. Defaults to a parameter with the value "admin".</param>
    /// <param name="secretKey">Optional secret-key parameter. Defaults to a generated password.</param>
    public static IResourceBuilder<SeaweedFsResource> AddSeaweedFs(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        int? s3Port = null,
        int? masterPort = null,
        IResourceBuilder<ParameterResource>? accessKey = null,
        IResourceBuilder<ParameterResource>? secretKey = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var accessKeyParameter = accessKey?.Resource
            ?? builder.AddParameter($"{name}-access-key", "admin").Resource;

        var secretKeyParameter = secretKey?.Resource
            ?? ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(
                builder, $"{name}-secret-key", special: false);

        var resource = new SeaweedFsResource(name, accessKeyParameter, secretKeyParameter);

        return builder.AddResource(resource)
            .WithImage(SeaweedFsContainerImageTags.Image)
            .WithImageRegistry(SeaweedFsContainerImageTags.Registry)
            .WithImageTag(SeaweedFsContainerImageTags.Tag)
            .WithHttpEndpoint(
                targetPort: SeaweedFsResource.S3TargetPort,
                port: s3Port,
                name: SeaweedFsResource.S3EndpointName)
            .WithHttpEndpoint(
                targetPort: SeaweedFsResource.MasterTargetPort,
                port: masterPort,
                name: SeaweedFsResource.MasterEndpointName)
            .WithContainerFiles(
                SeaweedFsResource.GeneratedConfigDirectory,
                (_, ct) => GenerateS3ConfigAsync(resource, ct))
            .WithArgs(context =>
            {
                var configPath = resource.CustomS3ConfigHostPath is null
                    ? $"{SeaweedFsResource.GeneratedConfigDirectory}/{SeaweedFsResource.GeneratedConfigFileName}"
                    : SeaweedFsResource.CustomConfigPath;

                context.Args.Add("server");
                context.Args.Add("-s3");
                context.Args.Add($"-s3.port={SeaweedFsResource.S3TargetPort}");
                context.Args.Add($"-s3.config={configPath}");
                context.Args.Add($"-dir={DataPath}");
            })
            .WithHttpHealthCheck("/cluster/status", endpointName: SeaweedFsResource.MasterEndpointName);
    }

    /// <summary>Persists <c>/data</c> in a named volume.</summary>
    public static IResourceBuilder<SeaweedFsResource> WithDataVolume(
        this IResourceBuilder<SeaweedFsResource> builder,
        string? name = null,
        bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), DataPath, isReadOnly);
    }

    /// <summary>Persists <c>/data</c> in a host directory.</summary>
    public static IResourceBuilder<SeaweedFsResource> WithDataBindMount(
        this IResourceBuilder<SeaweedFsResource> builder,
        string source,
        bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);
        return builder.WithBindMount(source, DataPath, isReadOnly);
    }

    /// <summary>Overrides the S3 access key.</summary>
    public static IResourceBuilder<SeaweedFsResource> WithAccessKey(
        this IResourceBuilder<SeaweedFsResource> builder,
        IResourceBuilder<ParameterResource> accessKey)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(accessKey);
        builder.Resource.AccessKeyParameter = accessKey.Resource;
        return builder;
    }

    /// <summary>Overrides the S3 secret key.</summary>
    public static IResourceBuilder<SeaweedFsResource> WithSecretKey(
        this IResourceBuilder<SeaweedFsResource> builder,
        IResourceBuilder<ParameterResource> secretKey)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(secretKey);
        builder.Resource.SecretKeyParameter = secretKey.Resource;
        return builder;
    }

    /// <summary>
    /// Uses your own s3.json (mounted read-only) instead of the generated identity config.
    /// Set the access/secret key to match an identity defined in that file.
    /// </summary>
    public static IResourceBuilder<SeaweedFsResource> WithS3ConfigFile(
        this IResourceBuilder<SeaweedFsResource> builder,
        string configFilePath)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(configFilePath);

        builder.Resource.CustomS3ConfigHostPath = configFilePath;
        return builder.WithBindMount(configFilePath, SeaweedFsResource.CustomConfigPath, isReadOnly: true);
    }

    /// <summary>
    /// Adds the SeaweedFS Admin UI container, connected to this resource's master.
    /// </summary>
    public static IResourceBuilder<SeaweedFsResource> WithSeaweedAdmin(
        this IResourceBuilder<SeaweedFsResource> builder,
        Action<IResourceBuilder<SeaweedFsAdminResource>>? configure = null,
        string? containerName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var parent = builder.Resource;

        if (builder.ApplicationBuilder.Resources
                .OfType<SeaweedFsAdminResource>()
                .Any(r => ReferenceEquals(r.Parent, parent)))
        {
            throw new DistributedApplicationException(
                $"The SeaweedFS resource '{parent.Name}' already has an admin UI.");
        }

        var admin = new SeaweedFsAdminResource(containerName ?? $"{parent.Name}-admin", parent);

        var adminBuilder = builder.ApplicationBuilder.AddResource(admin)
            .WithImage(SeaweedFsContainerImageTags.Image)
            .WithImageRegistry(SeaweedFsContainerImageTags.Registry)
            .WithImageTag(SeaweedFsContainerImageTags.Tag)
            .WithHttpEndpoint(
                targetPort: SeaweedFsAdminResource.AdminTargetPort,
                name: SeaweedFsAdminResource.HttpEndpointName)
            .WithArgs(context =>
            {
                context.Args.Add("admin");
                context.Args.Add($"-port={SeaweedFsAdminResource.AdminTargetPort}");
                // Resolves to "<master-container>:9333" on the container network.
                context.Args.Add(ReferenceExpression.Create(
                    $"-masters={parent.MasterEndpoint.Property(EndpointProperty.HostAndPort)}"));
            })
            .WithParentRelationship(parent)
            .WaitFor(builder);

        configure?.Invoke(adminBuilder);

        return builder;
    }

    private static async Task<IEnumerable<ContainerFileSystemItem>> GenerateS3ConfigAsync(
        SeaweedFsResource resource,
        CancellationToken cancellationToken)
    {
        // A user-supplied file is bind-mounted instead; nothing to generate.
        if (resource.CustomS3ConfigHostPath is not null)
        {
            return [];
        }

        var accessKey = await resource.AccessKeyParameter.GetValueAsync(cancellationToken).ConfigureAwait(false);
        var secretKey = await resource.SecretKeyParameter.GetValueAsync(cancellationToken).ConfigureAwait(false);

        var config = new
        {
            identities = new[]
            {
                new
                {
                    name = "aspire",
                    credentials = new[] { new { accessKey, secretKey } },
                    actions = new[] { "Admin", "Read", "Write", "List", "Tagging" }
                }
            }
        };

        return
        [
            new ContainerFile
            {
                Name = SeaweedFsResource.GeneratedConfigFileName,
                Contents = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true })
            }
        ];
    }
}
