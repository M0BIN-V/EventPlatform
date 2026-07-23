using BuildingBlocks.Application.Contracts;
using BuildingBlocks.Application.Events;
using BuildingBlocks.Infrastructure;
using DiServiceInstaller;
using Identity.Application.Features.Register;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Model;
using Notification.Application.Features;
using Polly.CircuitBreaker;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;

namespace WebApi.ServiceInstallers;

public class WolverineInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        var webAppBuilder = (WebApplicationBuilder)builder;

        webAppBuilder.Host.UseWolverine(opts =>
        {
            opts.OnException<BrokenCircuitException>()
                .RetryWithCooldown(5.Minutes(), 15.Minutes(), 30.Minutes());

            opts.PersistMessagesWithPostgresql(
                builder.Configuration.GetConnectionString("event-platform-db") ??
                throw new NullReferenceException("Connection string 'event-platform-db' is not configured"),
                "wolverine");
            opts.UseEntityFrameworkCoreTransactions();
            opts.Policies.UseDurableLocalQueues();
            opts.Policies.AutoApplyTransactions();

            opts.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;

            opts.Discovery.IncludeAssembly(typeof(ConfirmEmailRequestedEvent).Assembly);
            opts.Discovery.IncludeAssembly(typeof(RegisterHandler).Assembly);
            opts.Discovery.IncludeAssembly(typeof(ConfirmEmailRequestedEventHandler).Assembly);

            if (builder.Environment.IsDevelopment())
            {
                opts.CodeGeneration.GeneratedCodeOutputPath =
                    Path.Combine(builder.Environment.ContentRootPath, "obj", "Wolverine");
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