namespace Files.Application.Common.Contracts.Services;

public interface IObjectStorageService
{
    Task<PresignedUploadResponse> CreatePresignedUploadAsync(
        string objectName,
        DateTimeOffset expiresIn,
        long minLength,
        long maxLength);

    Task<bool> ObjectExistsAsync(string objectKey);

    Task<ObjectMetadataDto?> GetObjectMetadataAsync(string objectKey);

    Task<long?> GetObjectSizeAsync(string objectKey);

    Task<PresignedDownloadResponse> CreatePresignedDownloadUrlAsync(string objectKey, TimeSpan expiresIn);

    Task DeleteObjectAsync(string objectKey);
}