using BuildingBlocks.Application;
using Files.Application;
using Files.Application.Features.CreateUpload;
using Files.Contracts.Features.CreateUpload;
using Files.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Files.Presentation;

public static class ModuleInstaller
{
    public static IHostApplicationBuilder AddFilesModule(this IHostApplicationBuilder builder)
    {
        builder.AddFilesModuleApplication()
            .AddFilesModuleInfrastructure();

        builder.Services.AddScoped<Handler<CreateUploadRequest, CreateUploadResponse>, CreateUploadHandler>();

        return builder;
    }
}