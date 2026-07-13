using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("event-platform-postgres")
    .WithImage("postgis/postgis", "latest")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(config => config
        .WithContainerName("event-platform-pgadmin")
        .WithImage("dpage/pgadmin4", "9.9.0")
        .WithLifetime(ContainerLifetime.Persistent));

var database = postgres.AddDatabase("event-platform-db");

var mailpit = builder.AddMailPit("event-platform-mailpit")
    .WithLifetime(ContainerLifetime.Persistent);

var api = builder
    .AddProject<WebApi>("event-platform-api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(mailpit)
    .WaitFor(mailpit)
    .WithEnvironment(context =>
    {
        if (!builder.Environment.IsDevelopment()) return;
        var smtpEndpoint = mailpit.GetEndpoint("smtp");

        context.EnvironmentVariables["EmailSettings__SmtpServer"] = smtpEndpoint.Host;
        context.EnvironmentVariables["EmailSettings__Port"] = smtpEndpoint.Property(EndpointProperty.Port);
        context.EnvironmentVariables["EmailSettings__Username"] = "";
        context.EnvironmentVariables["EmailSettings__Password"] = "";
        context.EnvironmentVariables["EmailSettings__EnableSsl"] = "false";
        context.EnvironmentVariables["EmailSettings__DefaultFromEmail"] = "noreply@eventplatform.local";
        context.EnvironmentVariables["EmailSettings__DefaultFromName"] = "Event Platform";
        context.EnvironmentVariables["EmailSettings__Security"] = "Auto";
    });

// var webapp = builder
//     .AddProject<WebApp>("event-platform-webapp")
//     .WithReference(api)
//     .WaitFor(api);

builder.Build().Run();