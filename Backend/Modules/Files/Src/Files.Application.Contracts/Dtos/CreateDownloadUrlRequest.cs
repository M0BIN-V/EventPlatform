namespace Files.Application.Contracts.Dtos;

public record CreateDownloadUrlRequest(Guid FileId, TimeSpan? ExpiresIn = null);