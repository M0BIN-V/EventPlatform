using Testcontainers.PostgreSql;

namespace Shared.IntegrationTests.Common;

public class IntegrationTestFixture : IAsyncLifetime
{
    private PostgreSqlContainer Container { get; } = new PostgreSqlBuilder("postgres:18.3")
        .WithDatabase("event-platform-db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public WebApiFactory Api { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        Api = new WebApiFactory(Container);
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
    }
}