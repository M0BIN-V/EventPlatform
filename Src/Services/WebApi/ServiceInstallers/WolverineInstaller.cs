using BuildingBlocks.Application.Contracts;
using BuildingBlocks.Application.Events;
using BuildingBlocks.Infrastructure;
using DiServiceInstaller;
using Identity.Application.Features.Register;
using JasperFx.CodeGeneration;
using Notification.Application.Features;
using Wolverine;

namespace WebApi.ServiceInstallers;

public class WolverineInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        var webAppBuilder = (WebApplicationBuilder)builder;

        webAppBuilder.Host.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(ConfirmEmailRequestedEvent).Assembly);
            opts.Discovery.IncludeAssembly(typeof(RegisterHandler).Assembly);
            opts.Discovery.IncludeAssembly(typeof(ConfirmEmailRequestedEventHandler).Assembly);

            if (builder.Environment.IsDevelopment())
            {
                opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
                opts.UseRuntimeCompilation();
            }
            else
            {
                opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
            }
        });

        builder.Services.AddScoped<IMessagePublisher, WolverineMessagePublisher>();
    }
}