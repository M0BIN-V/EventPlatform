using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Files.Application.Common.Contracts.Services;
using Files.Contracts.Common.Enums;
using Files.Contracts.Dtos;
using Files.Infrastructure.ServiceInstallers;
using Microsoft.Extensions.Options;

namespace Files.Infrastructure.Storage;

public class S3ObjectStorageService(IAmazonS3 client, IOptions<S3ConnectionOptions> options) : IObjectStorageService
{
    public async Task<bool> ObjectExistsAsync(string objectKey)
    {
        try
        {
            var response = await client.GetObjectMetadataAsync(options.Value.Bucket, objectKey);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<long?> GetObjectSizeAsync(string objectKey)
    {
        var meta = await GetObjectMetadataAsync(objectKey);
        return meta?.Size;
    }

    public async Task DeleteObjectAsync(string objectKey)
    {
        await client.DeleteObjectAsync(options.Value.Bucket, objectKey);
    }

    public async Task<ObjectMetadataDto?> GetObjectMetadataAsync(string objectKey)
    {
        try
        {
            var meta = await client.GetObjectMetadataAsync(options.Value.Bucket, objectKey);
            var dict = new Dictionary<string, string>();
            foreach (var key in meta.Metadata.Keys) dict[key] = meta.Metadata[key];

            return new ObjectMetadataDto(dict, meta.ContentLength, meta.LastModified ?? DateTime.UtcNow);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<PresignedDownloadResponse> CreatePresignedDownloadUrlAsync(string objectKey, TimeSpan expiresIn)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.Value.Bucket ?? throw new InvalidOperationException("Bucket is not configured"),
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiresIn)
        };

        var url = await client.GetPreSignedURLAsync(request);
        return new PresignedDownloadResponse(url, DateTime.UtcNow.Add(expiresIn));
    }

    public async Task<PresignedUploadResponse> CreatePresignedUploadAsync(
        string objectName,
        DateTimeOffset expiresIn,
        FilePurpose purpose,
        long minLength,
        long maxLength)
    {
        var request = new CreatePresignedPostRequest
        {
            BucketName = options.Value.Bucket,
            Key = $"{purpose}/{objectName}",
            Expires = expiresIn.DateTime,
            Fields =
            {
                ["Content-Type"] = "application/octet-stream"
            }
        };

        request.Conditions.Add(new ContentLengthRangeCondition(minLength, maxLength));


        var result = await client.CreatePresignedPostAsync(request);

        var url = result.Url!;

        return new PresignedUploadResponse(url, objectName);
    }
}