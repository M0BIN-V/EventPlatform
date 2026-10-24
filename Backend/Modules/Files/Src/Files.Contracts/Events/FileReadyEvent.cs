namespace Files.Contracts.Events;

public record FileReadyEvent(
    Guid FileId,
    Guid? OrganizationId,
    string FileName,
    string ContentType,
    long Size);