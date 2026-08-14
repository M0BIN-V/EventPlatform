using Microsoft.Extensions.Hosting;
using Organizations.Application;
using Organizations.Infrastructure;

namespace Organizations.Presentation;

public static class ModuleInstaller
{
    public static IHostApplicationBuilder AddOrganizationModule(this IHostApplicationBuilder builder)
    {
        builder
            .AddOrganizationModuleApplication()
            .AddOrganizationModuleInfrastructure();

        return builder;
    }
}