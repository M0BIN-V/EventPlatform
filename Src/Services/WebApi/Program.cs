using Infrastructure;
using Infrastructure.Persistence;
using Presentation;
using Scalar.AspNetCore;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddIdentityServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.EnsureMigrationsApplied<EfIdentityDbContext>();
}

app.UseHttpsRedirection();

app.MapIdentityEndpoints();

app.UseAuthentication();

app.UseAuthorization();

app.Run();