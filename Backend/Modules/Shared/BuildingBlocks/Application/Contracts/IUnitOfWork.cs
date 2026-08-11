namespace BuildingBlocks.Application.Contracts;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}