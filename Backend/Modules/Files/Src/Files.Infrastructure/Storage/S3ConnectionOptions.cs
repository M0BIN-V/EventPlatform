using System.Data.Common;

namespace Files.Infrastructure.Storage;

public class S3ConnectionOptions
{
    public required string Endpoint { get; init; }

    public required string AccessKey { get; init; }

    public required string SecretKey { get; init; }

    public required string Bucket { get; init; }

    public static S3ConnectionOptions Parse(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        return new S3ConnectionOptions
        {
            Endpoint = GetRequired(builder, "Endpoint"),
            AccessKey = GetRequired(builder, "AccessKey"),
            SecretKey = GetRequired(builder, "SecretKey"),
            Bucket = "App"
        };
    }

    private static string GetRequired(DbConnectionStringBuilder builder, string key)
    {
        if (!builder.TryGetValue(key, out var value) ||
            string.IsNullOrWhiteSpace(value.ToString()))
            throw new InvalidOperationException($"RustFS connection string is missing required key '{key}'.");

        return value.ToString()!;
    }
}