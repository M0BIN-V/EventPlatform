namespace Files.Application.Contracts.Dtos;

public record CreateUploadResponse(
    Guid UploadSessionId,
    string Url,
    IDictionary<string, string>? Fields,
    DateTime ExpiresAt,
    string ObjectKey);