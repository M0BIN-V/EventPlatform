using Files.Contracts.Common.Enums;

namespace Files.Contracts.Dtos;

public record CreateUploadRequest(
    string FileName,
    string ContentType,
    long MinLength,
    long MaxLength,
    string[] PathSegments,
    ModuleName ModuleName
);