using Identity.Application;
using Identity.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Identity.Presentation;

public static class ModuleInstaller
{
    public static IHostApplicationBuilder AddIdentityModule(this IHostApplicationBuilder builder)
    {
        builder.AddIdentityModuleApplication()
            .AddIdentityModuleInfrastructure();

        return builder;
    }
}