namespace Files.Application.Common.Contracts.Persistence;

public interface IStorageService
{
    public Task<UploadTarget> CreateUploadTargetAsync(string key,
        string contentType,
        long contentLength,
        TimeSpan expiration);
}

public sealed record UploadTarget(
    string Url,
    IReadOnlyDictionary<string, string> RequiredHeaders,
    DateTime ExpiresAtUtc);