namespace AppHost.RustFs;

[AspireExport]
public sealed class RustFsResource(
    [ResourceName] string name,
    ParameterResource accessKey,
    ParameterResource secretKey)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string S3EndpointName = "s3";
    internal const string ConsoleEndpointName = "console";
    
    private EndpointReference? _consoleEndpoint;
    private EndpointReference? _s3Endpoint;

    public ParameterResource AccessKey { get; } = accessKey;

    public ParameterResource SecretKey { get; } = secretKey;

    public EndpointReference S3Endpoint =>
        _s3Endpoint ??= new EndpointReference(this, S3EndpointName);

    public EndpointReference ConsoleEndpoint =>
        _consoleEndpoint ??= new EndpointReference(this, ConsoleEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"Endpoint=http://{S3Endpoint.Property(EndpointProperty.HostAndPort)};" +
            $"AccessKey={AccessKey};" +
            $"SecretKey={SecretKey}");
}