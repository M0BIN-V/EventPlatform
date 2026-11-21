namespace Files.Domain.Entities;

public class FilePolicy
{
    public required bool Private { get; init; }

    public required List<string> ValidContentTypes { get; init; }
    public required long MinSize { get; init; }
    public required long MaxSize { get; init; }
}