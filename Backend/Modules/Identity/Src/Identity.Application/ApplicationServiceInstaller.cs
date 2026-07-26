using BuildingBlocks.Application.Extensions;
using Identity.Application.Common.Contracts.ApplicationServices;
using Identity.Application.Common.Options;
using Identity.Application.Common.Services;
using Identity.Application.Features.Register;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Application;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddIdentityModuleApplication(this IHostApplicationBuilder builder)
    {
        var assembly = typeof(ApplicationServiceInstaller).Assembly;

        builder.Services.RegisterHandlers(assembly);

        builder.Services.AddScoped<IRefreshTokenManager, RefreshTokenManager>();

        builder.Services.AddValidatorsFromAssembly(assembly);

        builder.Services.Configure<EmailConfirmationOptions>(
            builder.Configuration.GetSection("EmailConfirmationOptions"));


        builder.Services.Configure<RefreshTokenOptions>(
            builder.Configuration.GetSection("RefreshToken"));

        return builder;
    }
}