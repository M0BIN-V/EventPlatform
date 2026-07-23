using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Application;

public abstract class Handler<TRequest>
{
    public abstract Task Handle(TRequest request, CancellationToken ct);
}

public abstract class Handler<TRequest, TResponse>
{
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}