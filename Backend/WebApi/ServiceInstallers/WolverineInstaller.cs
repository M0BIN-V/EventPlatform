using BuildingBlocks.Application.Contracts;
using BuildingBlocks.Infrastructure;
using DiServiceInstaller;
using Identity.Application.Features.Register;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Model;
using Messaging;
using Notification.Application.Features;
using Polly.CircuitBreaker;
using WebApi.Extensions;
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

            var connectionString = builder.Configuration.GetConnectionString("event-platform-db");

            if (ProcessHelper.IsDesignTimeProcess())
                builder.Services
                    .DisableAllExternalWolverineTransports()
                    .DisableAllWolverineMessagePersistence();

            if (!ProcessHelper.IsDesignTimeProcess())
                opts.PersistMessagesWithPostgresql(
                    connectionString ?? throw new NullReferenceException("Connection string is not configured"),
                    "wolverine");

            opts.UseEntityFrameworkCoreTransactions();
            opts.Policies.UseDurableLocalQueues();
            opts.Policies.AutoApplyTransactions();

            opts.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;

            opts.Discovery.IncludeAssembly(typeof(ConfirmEmailRequestedEvent).Assembly);
            opts.Discovery.IncludeAssembly(typeof(RegisterHandler).Assembly);
            opts.Discovery.IncludeAssembly(typeof(ConfirmEmailRequestedEventHandler).Assembly);

            opts.CodeGeneration.GeneratedCodeOutputPath =
                Path.Combine(builder.Environment.ContentRootPath,"Generated","Wolverine");

            opts.CodeGeneration.TypeLoadMode = builder.Environment.IsDevelopment()
                ? TypeLoadMode.Auto
                : TypeLoadMode.Static;
        });

        builder.Services.AddScoped<IMessagePublisher, WolverineMessagePublisher>();
    }
}