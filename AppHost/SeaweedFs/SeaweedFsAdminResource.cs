namespace AppHost.SeaweedFs;

/// <summary>
///     The SeaweedFS Admin UI (<c>weed admin</c>) container. Deliberately does not implement IResourceWithParent,
///     because Aspire forbids WaitFor on a parent of such a resource.
/// </summary>
public sealed class SeaweedFsAdminResource : ContainerResource
{
    internal const string HttpEndpointName = "http";
    internal const int AdminTargetPort = 23646;

    public SeaweedFsAdminResource([ResourceName] string name, SeaweedFsResource parent) : base(name)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    public SeaweedFsResource Parent { get; }
}