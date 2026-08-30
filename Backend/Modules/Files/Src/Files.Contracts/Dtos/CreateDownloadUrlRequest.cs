namespace Files.Contracts.Dtos;

public record CreateDownloadUrlRequest(Guid FileId, TimeSpan? ExpiresIn = null);