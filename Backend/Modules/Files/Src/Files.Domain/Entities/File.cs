namespace Files.Domain.Entities;

public enum FilePurpose
{
    Unknown = 0,
    OrganizationLogo = 1
}

public enum FileStatus
{
    Pending = 0,
    Ready = 1,
    Failed = 2
}

public class File
{
    private File(string objectKey, string fileName, string contentType, FilePurpose purpose)
    {
        Id = Guid.CreateVersion7();
        ObjectKey = objectKey;
        FileName = fileName;
        ContentType = contentType;
        Purpose = purpose;
        Status = FileStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string ObjectKey { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public long? Size { get; private set; }
    public FilePurpose Purpose { get; private set; }
    public FileStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }

    // Factory for creating a new pending file (upload session created)
    public static File CreatePending(string objectKey, string fileName, string contentType,
        FilePurpose purpose = FilePurpose.OrganizationLogo)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
            throw new ArgumentException("objectKey is required", nameof(objectKey));

        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("fileName is required", nameof(fileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("contentType is required", nameof(contentType));

        return new File(objectKey, fileName, contentType, purpose);
    }

    public void MarkReady(long size)
    {
        if (Status != FileStatus.Pending)
            throw new InvalidOperationException($"Cannot mark file ready from status {Status}");

        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));

        Size = size;
        Status = FileStatus.Ready;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        if (Status == FileStatus.Ready)
            throw new InvalidOperationException("Cannot mark a ready file as failed");

        Status = FileStatus.Failed;
        FailureReason = reason;
        CompletedAt = DateTime.UtcNow;
    }
}