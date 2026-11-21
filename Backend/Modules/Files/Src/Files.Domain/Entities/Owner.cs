namespace Files.Domain.Entities;

public class Owner
{
    public required string Id { get; init; }

    public required List<Purpose> Purposes { get; init; }
}