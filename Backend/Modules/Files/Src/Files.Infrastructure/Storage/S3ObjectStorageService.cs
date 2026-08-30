using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;
using Files.Application.Common.Contracts.Services;
using Files.Application.Contracts.Dtos;
using Files.Infrastructure.Storage;

namespace Files.Infrastructure.Storage;

public class S3ObjectStorageService : IObjectStorageService
{
    private readonly IAmazonS3 _client;
    private readonly FilesStorageOptions _options;

    public S3ObjectStorageService(IAmazonS3 client, IOptions<FilesStorageOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<PresignedUploadResponse> CreatePresignedUploadAsync(string objectKey, TimeSpan expiresIn, IDictionary<string,string>? metadata = null)
    {
        // Use pre-signed PUT URL (S3-compatible)
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket ?? throw new InvalidOperationException("Bucket is not configured"),
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.Add(expiresIn)
        };

        var url = _client.GetPreSignedURL(request);

        return new PresignedUploadResponse(url, null, DateTime.UtcNow.Add(expiresIn), objectKey);
    }

    public async Task<bool> ObjectExistsAsync(string objectKey)
    {
        try
        {
            var response = await _client.GetObjectMetadataAsync(_options.Bucket!, objectKey);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<ObjectMetadataDto?> GetObjectMetadataAsync(string objectKey)
    {
        try
        {
            var meta = await _client.GetObjectMetadataAsync(_options.Bucket, objectKey);
            var dict = new Dictionary<string,string>();
            foreach (var key in meta.Metadata.Keys)
            {
                dict[key] = meta.Metadata[key];
            }

            return new ObjectMetadataDto(dict, meta.ContentLength, meta.LastModified ?? DateTime.UtcNow);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<long?> GetObjectSizeAsync(string objectKey)
    {
        var meta = await GetObjectMetadataAsync(objectKey);
        return meta?.Size;
    }

    public async Task<PresignedDownloadResponse> CreatePresignedDownloadUrlAsync(string objectKey, TimeSpan expiresIn)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket ?? throw new InvalidOperationException("Bucket is not configured"),
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiresIn)
        };

        var url = _client.GetPreSignedURL(request);
        return new PresignedDownloadResponse(url, DateTime.UtcNow.Add(expiresIn));
    }

    public async Task DeleteObjectAsync(string objectKey)
    {
        await _client.DeleteObjectAsync(_options.Bucket, objectKey);
    }
}
