namespace BuildingBlocks.Infrastructure;

public interface IModuleInitializer
{
    Task InitializeAsync(CancellationToken ct = default);
}