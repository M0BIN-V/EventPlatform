using AppHost.Extensions;
using JasperFx.Aspire;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("event-platform-postgres")
    .WithImage("postgis/postgis", "latest")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(config => config
        .WithContainerName("event-platform-pgadmin")
        .WithImage("dpage/pgadmin4", "9.9.0"));

var database = postgres.AddDatabase("event-platform-db");

var mailpit = builder.AddMailPit("event-platform-mailpit")
    .WithLifetime(ContainerLifetime.Persistent);

var api = builder
    .AddProject<WebApi>("event-platform-api")
    .WithJasperFxCommands(opts => { opts.IncludeMutatingCommands = true; })
    .WithReference(database)
    .WaitFor(database)
    .WithReference(mailpit)
    .WaitFor(mailpit)
    .ConfigureMailSettings(mailpit);

var identityModuleMigration = api
    .AddEFMigrations("identity-module-migrations", "Identity.Infrastructure.Persistence.DbContext.EfIdentityDbContext")
    .RunDatabaseUpdateOnStart()
    .WaitFor(database)
    .WaitFor(mailpit);

// var webapp = builder
//     .AddProject<WebApp>("event-platform-webapp")
//     .WithReference(api)
//     .WaitFor(api);

builder.Build().Run();