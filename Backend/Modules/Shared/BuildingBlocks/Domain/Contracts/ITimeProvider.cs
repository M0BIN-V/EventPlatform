namespace BuildingBlocks.Domain.Contracts;

public interface ITimeProvider
{
    DateTimeOffset Now { get; }
}