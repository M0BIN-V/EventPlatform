namespace BuildingBlocks.Application.Contracts;

public interface IDbContextBase
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}