using Files.Application.Contracts.Dtos;

namespace Files.Application.Contracts.Common.Contracts.ApplicationServices;

public interface IFilesService
{
    Task<CreateUploadResponse> CreateUploadAsync(CreateUploadRequest request);

    Task<CreateDownloadUrlResponse> CreateDownloadUrlAsync(CreateDownloadUrlRequest request);
}