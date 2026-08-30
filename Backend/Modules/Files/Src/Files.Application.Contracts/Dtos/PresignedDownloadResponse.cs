using System;

namespace Files.Application.Contracts.Dtos;

public record PresignedDownloadResponse(string Url, DateTime ExpiresAt);
