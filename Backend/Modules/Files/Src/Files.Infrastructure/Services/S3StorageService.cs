using Amazon.S3;
using Amazon.S3.Model;
using Files.Application.Common.Contracts.Persistence;
using Microsoft.Extensions.Options;

namespace Files.Infrastructure.Services;

public class S3StorageService(
    IOptions<S3ConnectionOptions> options,
    TimeProvider timeProvider,
    IAmazonS3 s3Client) : IStorageService
{
    public async Task<UploadTarget> CreateUploadTargetAsync(string key,
        string contentType,
        long contentLength,
        TimeSpan expiration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contentLength);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.Value.Bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = timeProvider.GetUtcNow().Add(expiration).DateTime,
            ContentType = contentType,
            Headers =
            {
                ContentLength = contentLength,
                ["If-None-Match"] = "*"
            }
        };

        var url = await s3Client.GetPreSignedURLAsync(request);

        var headers = new Dictionary<string, string>
        {
            ["Content-Type"] = contentType,
            ["If-None-Match"] = "*"
            // Content-Length will be set by browser or client 
        };

        return new UploadTarget(url, headers, request.Expires.Value);
    }
}