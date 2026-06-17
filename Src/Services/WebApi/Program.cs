using DiServiceInstaller;
using Endpoints;
using Infrastructure.Persistence;
using Scalar.AspNetCore;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

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