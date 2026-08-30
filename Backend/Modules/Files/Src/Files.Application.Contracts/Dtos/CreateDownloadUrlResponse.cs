namespace Files.Application.Contracts.Dtos;

public record CreateDownloadUrlResponse(string Url, DateTime ExpiresAt);