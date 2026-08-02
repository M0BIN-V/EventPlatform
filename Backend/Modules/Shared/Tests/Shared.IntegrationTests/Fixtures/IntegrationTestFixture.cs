using System.Data.Common;
using BuildingBlocks.Infrastructure;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Identity.Infrastructure.Persistence.DbContext;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notification.Infrastructure.Options;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Wolverine.Tracking;

namespace Shared.IntegrationTests.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly IContainer _mailpit = new ContainerBuilder("axllent/mailpit:latest")
        .WithPortBinding(1025, true) // SMTP
        .WithPortBinding(8025, true) // Web UI
        .Build();

    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:18.3")
        .WithName("test-container-postgres")
        .WithDatabase("event-platform-db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private DbConnection _connection = null!;
    private Respawner _respawner = null!;
    public WebApiFactory WebApiFactory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var dbConnectionString = _postgresContainer.GetConnectionString();

        await _mailpit.StartAsync();
        var emailOptions = new EmailOptions
        {
            SmtpServer = _mailpit.Hostname,
            Port = _mailpit.GetMappedPublicPort(1025),
            Username = "",
            Password = "",
            DefaultFromName = "Event Platform",
            DefaultFromEmail = "noreply@eventplatform.local",
            Security = SecureSocketOptions.Auto
        };

        WebApiFactory = new WebApiFactory(dbConnectionString, emailOptions);

        var options = new DbContextOptionsBuilder<EfIdentityDbContext>()
            .UseNpgsql(dbConnectionString)
            .Options;
        await using var db = new EfIdentityDbContext(options);
        await db.Database.MigrateAsync();

        _connection = new NpgsqlConnection(dbConnectionString);

        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres
        });
    }

    public async Task DisposeAsync()
    {
        await WebApiFactory.DisposeAsync();

        await _connection.DisposeAsync();

        await _mailpit.StopAsync();

        await _postgresContainer.StopAsync();
    }

    public async Task<ITrackedSession> TrackWolverineAsync(Func<Task> action)
    {
        return await WebApiFactory.Services.ExecuteAndWaitAsync(action);
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connection);


        using var scope = WebApiFactory.Services.CreateScope();
        var moduleInitializers = scope.ServiceProvider
            .GetServices<IModuleInitializer>();

        foreach (var initializer in moduleInitializers) await initializer.InitializeAsync();
    }

    public HttpClient CreateClient()
    {
        var client = WebApiFactory.CreateClient();

        return client;
    }
}