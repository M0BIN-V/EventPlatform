using Files.Contracts.Dtos;

namespace Files.Contracts.Common.Contracts.Services;

public interface IFilesService
{
    Task<CreateUploadResponse> CreateUploadAsync(CreateUploadRequest request);

    Task<CreateDownloadUrlResponse> CreateDownloadUrlAsync(CreateDownloadUrlRequest request);
}