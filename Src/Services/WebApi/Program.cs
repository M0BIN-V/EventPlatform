using DiServiceInstaller;
using Identity.Infrastructure.Persistence.DbContext;
using Identity.Presentation;
using JasperFx;
using Scalar.AspNetCore;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.InstallServices(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.Theme = ScalarTheme.BluePlanet);

    await app.EnsureMigrationsApplied<EfIdentityDbContext>();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();

var apiGroup = app.MapGroup("api");
apiGroup.MapIdentityModuleEndpoints();

return await app.RunJasperFxCommands(args);