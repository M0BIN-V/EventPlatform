using Files.Contracts.Common.Enums;
using Files.Domain.Constants;

namespace Files.Domain.Entities;

public class File
{
    private File()
    {
    }

    public Guid Id { get; private set; }

    public string? ObjectKey { get; private set; }
    public string? FileName { get; private set; }
    public string? ContentType { get; private set; }
    public long? Size { get; private set; }
    public ModuleName ModuleName { get; private set; }
    public FileStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UploadedAt { get; private set; }
    public DateTimeOffset? ReadyAt { get; private set; }

    public static File CreatePending(string objectKey,ModuleName moduleName, DateTimeOffset createdAt)
    {
        return new File
        {
            ObjectKey = objectKey,
            CreatedAt = createdAt,
            ModuleName = moduleName,
            Status = FileStatus.Pending
        };
    }

    public void MarkUploaded(
        string fileName,
        string contentType,
        long size,
        DateTimeOffset uploadedAt)
    {
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        UploadedAt = uploadedAt;
        Status = FileStatus.Uploaded;
    }

    public void MarkReady(DateTimeOffset readyAt)
    {
        if (Status != FileStatus.Uploaded)
            throw new InvalidOperationException($"Cannot mark file ready from status {Status}");

        Status = FileStatus.Ready;
        ReadyAt = readyAt;
    }

    public void MarkFailed()
    {
        if (Status != FileStatus.Uploaded)
            throw new InvalidOperationException($"Cannot mark file failed from status {Status}");

        Status = FileStatus.Failed;
    }
}