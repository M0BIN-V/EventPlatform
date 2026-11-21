namespace Files.Contracts.Features.CreateUpload;

public record CreateUploadRequest(
    string OwnerId,
    string PurposeId,
    string FileName,
    string ContentType,
    long Size);