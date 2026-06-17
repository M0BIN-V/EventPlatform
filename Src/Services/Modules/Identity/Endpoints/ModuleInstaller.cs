using Application;
using Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Endpoints;

public static class ModuleInstaller
{
    public static IHostApplicationBuilder AddIdentityModule(this IHostApplicationBuilder builder)
    {
        builder.AddIdentityModuleApplication()
            .AddIdentityModuleInfrastructure();

        return builder;
    }
}