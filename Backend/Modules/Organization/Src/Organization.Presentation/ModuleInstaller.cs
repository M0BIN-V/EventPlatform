using Organization.Infrastructure;
using Organization.Application;
using Microsoft.Extensions.Hosting;

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
