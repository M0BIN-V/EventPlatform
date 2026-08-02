namespace BuildingBlocks.Infrastructure;

public interface IModuleInitializer
{
    public Task InitializeAsync(CancellationToken ct = default);
}