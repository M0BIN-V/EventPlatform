namespace Files.Domain.Entities;

public class Purpose
{
    public required string Id { get; init; }
    public required FilePolicy Policy { get; init; }
}