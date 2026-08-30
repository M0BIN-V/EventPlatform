using System;
using System.Threading.Tasks;
using Amazon.S3.Model;
using NSubstitute;
using Shouldly;
using Xunit;
using Files.Infrastructure.Storage;
using Microsoft.Extensions.Options;
using Amazon.S3;
using System.Collections.Generic;
using Files.Application.Contracts.Dtos;

namespace Files.Infrastructure.Tests.Storage;

public class S3ObjectStorageServiceTests
{
    [Fact]
    public async Task ObjectExistsAsync_ReturnsTrue_WhenObjectPresent()
    {
        var client = Substitute.For<IAmazonS3>();
        var bucket = "test-bucket";
        var objectKey = "files/1";

        client.GetObjectMetadataAsync(bucket, objectKey).Returns(Task.FromResult(new GetObjectMetadataResponse { ContentLength = 123, HttpStatusCode = System.Net.HttpStatusCode.OK }));

        var options = Options.Create(new FilesStorageOptions { Bucket = bucket });
        var svc = new S3ObjectStorageService(client, Options.Create(options.Value));

        var exists = await svc.ObjectExistsAsync(objectKey);
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task ObjectExistsAsync_ReturnsFalse_WhenNotFound()
    {
        var client = Substitute.For<IAmazonS3>();
        var bucket = "test-bucket";
        var objectKey = "files/1";

        var ex = new AmazonS3Exception("Not found") { StatusCode = System.Net.HttpStatusCode.NotFound };
        client.GetObjectMetadataAsync(bucket, objectKey).Returns<Task<GetObjectMetadataResponse>>(x => throw ex);

        var svc = new S3ObjectStorageService(client, Options.Create(new FilesStorageOptions { Bucket = bucket }));

        var exists = await svc.ObjectExistsAsync(objectKey);
        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task GetObjectMetadataAsync_ReturnsMetadata_WhenPresent()
    {
        var client = Substitute.For<IAmazonS3>();
        var bucket = "test-bucket";
        var objectKey = "files/1";

        var resp = new GetObjectMetadataResponse { ContentLength = 888, LastModified = DateTime.UtcNow, HttpStatusCode = System.Net.HttpStatusCode.OK };
        resp.Metadata.Add("x-amz-meta-custom", "value");

        client.GetObjectMetadataAsync(bucket, objectKey).Returns(Task.FromResult(resp));

        var svc = new S3ObjectStorageService(client, Options.Create(new FilesStorageOptions { Bucket = bucket }));

        var meta = await svc.GetObjectMetadataAsync(objectKey);

        meta.ShouldNotBeNull();
        meta!.Size.ShouldBe(888);
        meta.Metadata.ShouldContainKey("x-amz-meta-custom");
    }
}
