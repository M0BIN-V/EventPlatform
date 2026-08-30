using Files.Contracts.Common.Enums;

namespace Files.Contracts.Dtos;

public record CreateUploadRequest(
    string ContentType,
    long MinLength,
    long MaxLength,
    FilePurpose Purpose);