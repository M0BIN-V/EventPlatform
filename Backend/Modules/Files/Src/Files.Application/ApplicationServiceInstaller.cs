using BuildingBlocks.Application.Extensions;
using Microsoft.Extensions.Hosting;

namespace Files.Application;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddFilesModuleApplication(this IHostApplicationBuilder builder)
    {
        var assembly = typeof(ApplicationServiceInstaller).Assembly;

                builder.Services.RegisterHandlers(assembly);
        builder.Services.AddValidatorsFromAssembly(assembly);

                return builder;
    }
}
