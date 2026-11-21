using Files.Domain.Enums;

namespace Files.Domain.Entities;

public class File
{
    public Guid Id { get; init; }

    public required string ObjectKey { get; init; }
    public required string ContentType { get; init; }
    public required string FileName { get; init; }
    public required long Size { get; init; }
    public required string OwnerId { get; init; }
    public required string PurposeId { get; init; }
    public FileStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UploadedAt { get; set; }
    public DateTimeOffset? ReadyAt { get; set; }
}