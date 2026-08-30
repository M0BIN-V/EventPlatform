using System;

namespace Files.Contracts.Dtos;

public record PresignedDownloadResponse(string Url, DateTime ExpiresAt);
