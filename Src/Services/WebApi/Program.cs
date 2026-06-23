using DiServiceInstaller;
using Identity.Presentation;
using Infrastructure.Persistence;
using Scalar.AspNetCore;
using WebApi.Extensions;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWolverine(opt =>
{
    opt.Discovery.IncludeAssembly(typeof(BuildingBlocks.Application.Events.ConfirmEmailRequestedEvent).Assembly);
    opt.Discovery.IncludeAssembly(typeof(application))
});

builder.InstallServices(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    await app.EnsureMigrationsApplied<EfIdentityDbContext>();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityModuleEndpoints();

app.Run();