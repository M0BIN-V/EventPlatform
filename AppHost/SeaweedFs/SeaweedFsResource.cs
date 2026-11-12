// Resource types live in Aspire.Hosting.ApplicationModel for ease of discovery.

namespace AppHost.SeaweedFs;

/// <summary>
///     A SeaweedFS container running in "server" mode (master + volume + filer + S3 gateway).
/// </summary>
public sealed class SeaweedFsResource : ContainerResource, IResourceWithConnectionString
{
    internal const string S3EndpointName = "s3";
    internal const string MasterEndpointName = "master";

    internal const int S3TargetPort = 8333;
    internal const int MasterTargetPort = 9333;

    internal const string GeneratedConfigDirectory = "/etc/seaweedfs";
    internal const string GeneratedConfigFileName = "s3.json";
    internal const string CustomConfigPath = "/etc/seaweedfs-custom/s3.json";
    private EndpointReference? _masterReference;

    private EndpointReference? _s3Reference;

    public SeaweedFsResource(
        [ResourceName] string name,
        ParameterResource accessKey,
        ParameterResource secretKey) : base(name)
    {
        AccessKeyParameter = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
        SecretKeyParameter = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
    }

    /// <summary>S3 access key parameter.</summary>
    public ParameterResource AccessKeyParameter { get; internal set; }

    /// <summary>S3 secret key parameter.</summary>
    public ParameterResource SecretKeyParameter { get; internal set; }

    /// <summary>Host path of a user-supplied s3.json, if <c>WithS3ConfigFile</c> was used.</summary>
    internal string? CustomS3ConfigHostPath { get; set; }

    public EndpointReference S3Endpoint => _s3Reference ??= new EndpointReference(this, S3EndpointName);

    public EndpointReference MasterEndpoint => _masterReference ??= new EndpointReference(this, MasterEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"Endpoint={S3Endpoint.Property(EndpointProperty.Url)};AccessKey={AccessKeyParameter};SecretKey={SecretKeyParameter}");

    public IEnumerable<KeyValuePair<string, ReferenceExpression>> GetConnectionProperties()
    {
        yield return new KeyValuePair<string, ReferenceExpression>("Host",
            ReferenceExpression.Create($"{S3Endpoint.Property(EndpointProperty.Host)}"));
        yield return new KeyValuePair<string, ReferenceExpression>("Port",
            ReferenceExpression.Create($"{S3Endpoint.Property(EndpointProperty.Port)}"));
        yield return new KeyValuePair<string, ReferenceExpression>("S3Url",
            ReferenceExpression.Create($"{S3Endpoint.Property(EndpointProperty.Url)}"));
        yield return new KeyValuePair<string, ReferenceExpression>("MasterUrl",
            ReferenceExpression.Create($"{MasterEndpoint.Property(EndpointProperty.Url)}"));
        yield return new KeyValuePair<string, ReferenceExpression>("AccessKey",
            ReferenceExpression.Create($"{AccessKeyParameter}"));
        yield return new KeyValuePair<string, ReferenceExpression>("SecretKey",
            ReferenceExpression.Create($"{SecretKeyParameter}"));
    }
}