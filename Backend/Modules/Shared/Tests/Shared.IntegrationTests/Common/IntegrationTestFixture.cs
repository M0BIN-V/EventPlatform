using System.Data.Common;
using Identity.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Shared.IntegrationTests.Common;

public class IntegrationTestFixture : IAsyncLifetime
{
    private DbConnection _connection = null!;
    private Respawner _respawner = null!;
    private WebApiFactory _webApiFactory = null!;

    private PostgreSqlContainer PostgresContainer { get; } = new PostgreSqlBuilder("postgres:18.3")
        .WithName("haaaaaaaaaaaaaaaaaaa")
        .WithDatabase("event-platform-db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await PostgresContainer.StartAsync();
        _webApiFactory = new WebApiFactory(PostgresContainer);

        var identityDb = _webApiFactory.Services
            .CreateScope().ServiceProvider
            .GetRequiredService<EfIdentityDbContext>();

        await identityDb.Database.MigrateAsync();

        _connection = new NpgsqlConnection(PostgresContainer.GetConnectionString());

        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres
        });
    }

    public async Task DisposeAsync()
    {
        await PostgresContainer.StopAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connection);
    }

    public HttpClient CreateClient()
    {
        var client = _webApiFactory.CreateClient();

        return client;
    }
}