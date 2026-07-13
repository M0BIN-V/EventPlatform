using DiServiceInstaller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Application.Contracts.Services;
using Notification.Infrastructure.Services;

namespace Notification.Infrastructure;

public static class ModuleInstaller
{
    public static IHostApplicationBuilder AddNotificationModule(this IHostApplicationBuilder builder)
    {
        builder.InstallServices(typeof(ModuleInstaller).Assembly);
        
        builder.Services.AddTransient<IEmailService, MailkitEmailService>();

        return builder;
    }
}