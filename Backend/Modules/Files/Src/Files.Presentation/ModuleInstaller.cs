using Files.Application;
using Files.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Files.Presentation;

public static class ModuleInstaller
{
    public static IHostApplicationBuilder AddFilesModule(this IHostApplicationBuilder builder)
    {
        builder.AddFilesModuleApplication()
            .AddFilesModuleInfrastructure();

        return builder;
    }
}