namespace Files.Contracts.Events;

public record FileReadyEvent(
    Guid FileId,
    string OwnerUserId,
    Guid? OrganizationId,
    string FileName,
    string ContentType,
    long Size);