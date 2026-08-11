namespace BuildingBlocks.Application.Contracts;

public interface ICurrentUser
{
    Guid UserId { get; }
}