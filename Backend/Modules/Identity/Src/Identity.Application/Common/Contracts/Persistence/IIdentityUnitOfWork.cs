namespace Identity.Application.Common.Contracts.Persistence;

public interface IIdentityUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}