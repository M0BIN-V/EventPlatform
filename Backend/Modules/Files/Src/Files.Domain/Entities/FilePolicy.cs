namespace Files.Domain.Entities;

public class FilePolicy
{
    public required bool IsPrivate { get; init; }

    public required List<string> ValidContentTypes { get; init; }
    public required long MinSize { get; init; }
    public required long MaxSize { get; init; }
}