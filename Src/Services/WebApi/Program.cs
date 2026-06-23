using BuildingBlocks.Application.Events;
using DiServiceInstaller;
using Identity.Application.Features.Register;
using Identity.Infrastructure.Persistence;
using Identity.Presentation;
using JasperFx;
using Notification.Application.Features;
using Scalar.AspNetCore;
using WebApi.Extensions;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWolverine(opt =>
{
    opt.Discovery.IncludeAssembly(typeof(ConfirmEmailRequestedEvent).Assembly);
    opt.Discovery.IncludeAssembly(typeof(RegisterHandler).Assembly);
    opt.Discovery.IncludeAssembly(typeof(ConfirmEmailRequestedEventHandler).Assembly);
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

return await app.RunJasperFxCommands(args);