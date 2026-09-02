namespace AppHost.RustFs;

public class WebHookConfig(IResourceBuilder<RustFsResource> builder)
{
    public WebHookConfig SetEndpoint(Uri endpoint)
    {
        var origin = $"{endpoint.Scheme}://{endpoint.Authority}";

        builder.WithEnvironment("RUSTFS_OUTBOUND_ALLOW_ORIGINS", origin);

        builder.WithEnvironment("RUSTFS_NOTIFY_WEBHOOK_SKIP_TLS_VERIFY_PRIMARY", "true");

        builder.WithEnvironment("RUSTFS_NOTIFY_WEBHOOK_ENDPOINT_PRIMARY", endpoint.ToString());
        return this;
    }

    public WebHookConfig SetAuthToken(string token)
    {
        builder.WithEnvironment("RUSTFS_NOTIFY_WEBHOOK_AUTH_TOKEN_PRIMARY", token);
        return this;
    }


    public WebHookConfig SetQueueDirectory(string directory)
    {
        builder.WithEnvironment("RUSTFS_NOTIFY_WEBHOOK_QUEUE_DIR_PRIMARY", directory);
        return this;
    }

    public WebHookConfig SetQueueLimit(int limit)
    {
        builder.WithEnvironment("RUSTFS_NOTIFY_WEBHOOK_QUEUE_LIMIT_PRIMARY", limit.ToString());
        return this;
    }
}