using Files.Contracts.Common.Enums;

namespace Files.Application.Common.Contracts.Services;

public interface IObjectStorageService
{
    Task<PresignedUploadResponse> CreatePresignedUploadAsync(
        string objectName,
        DateTimeOffset expiresIn,
        FilePurpose purpose,
        long minLength,
        long maxLength);

    Task<bool> ObjectExistsAsync(string objectKey);

    Task<ObjectMetadataDto?> GetObjectMetadataAsync(string objectKey);

    Task<long?> GetObjectSizeAsync(string objectKey);

    Task<PresignedDownloadResponse> CreatePresignedDownloadUrlAsync(string objectKey, TimeSpan expiresIn);

    Task DeleteObjectAsync(string objectKey);
}