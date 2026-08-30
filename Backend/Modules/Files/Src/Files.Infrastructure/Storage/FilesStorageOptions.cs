namespace Files.Infrastructure.Storage;

public record FilesStorageOptions
{
    public string Provider { get; init; } = "RustFs"; // or S3
    public string? Endpoint { get; init; }
    public string? AccessKey { get; init; }
    public string? SecretKey { get; init; }
    public string? Bucket { get; init; }
    public string? Region { get; init; }
    public bool UsePathStyle { get; init; } = true;
}
