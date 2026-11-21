namespace Files.Domain.Enums;

public enum FileStatus
{
    Pending = 0,
    Uploaded = 1,
    Validating = 2,
    Processing = 3,
    Ready = 4,
    Rejected = 5
}