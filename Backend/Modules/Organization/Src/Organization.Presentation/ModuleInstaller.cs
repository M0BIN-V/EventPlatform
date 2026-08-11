using Microsoft.Extensions.Hosting;
using Organization.Application;
using Organization.Infrastructure;

namespace Organization.Presentation;

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