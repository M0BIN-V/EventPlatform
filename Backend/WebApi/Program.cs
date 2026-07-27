using DiServiceInstaller;
using Identity.Presentation;
using JasperFx;
using Scalar.AspNetCore;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

var isOpenApiBuild = AppDomain.CurrentDomain.RunByDocumentInsider();

if (!isOpenApiBuild)
{
    builder.AddServiceDefaults();
    builder.InstallServices(typeof(Program).Assembly);
}

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment() || isOpenApiBuild)
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.Theme = ScalarTheme.BluePlanet);

    //await app.EnsureMigrationsApplied<EfIdentityDbContext>();
}

if (!isOpenApiBuild)
{
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapDefaultEndpoints();
}

var apiGroup = app.MapGroup("api");
apiGroup.MapIdentityModuleEndpoints();

return await app.RunJasperFxCommands(args);