using BuildingBlocks.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Organization.Application;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddOrganizationModuleApplication(this IHostApplicationBuilder builder)
    {
        var assembly = typeof(ApplicationServiceInstaller).Assembly;

        builder.Services.RegisterHandlers(assembly);
        builder.Services.AddValidatorsFromAssembly(assembly);

        return builder;
    }
}
