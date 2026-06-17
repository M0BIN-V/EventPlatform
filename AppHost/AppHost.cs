using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("event-platform-postgres")
    .WithImage("postgis/postgis", "latest")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin(config => config
        .WithContainerName("event-platform-pgadmin")
        .WithImage("dpage/pgadmin4", "9.9.0")
        .WithLifetime(ContainerLifetime.Persistent));

var database = postgres.AddDatabase("event-platform-db");

var api = builder
    .AddProject<WebApi>("event-platform-api")
    .WithReference(database)
    .WaitFor(database);

// var webapp = builder
//     .AddProject<WebApp>("event-platform-webapp")
//     .WithReference(api)
//     .WaitFor(api);

builder.Build().Run();