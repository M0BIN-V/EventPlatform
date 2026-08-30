using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Files.Application.Contracts.Dtos;

namespace Files.Application.Common.Contracts.Services;

public interface IObjectStorageService
{
    Task<PresignedUploadResponse> CreatePresignedUploadAsync(string objectKey, TimeSpan expiresIn, IDictionary<string,string>? metadata = null);

    Task<bool> ObjectExistsAsync(string objectKey);

    Task<ObjectMetadataDto?> GetObjectMetadataAsync(string objectKey);

    Task<long?> GetObjectSizeAsync(string objectKey);

    Task<PresignedDownloadResponse> CreatePresignedDownloadUrlAsync(string objectKey, TimeSpan expiresIn);

    Task DeleteObjectAsync(string objectKey);
}
