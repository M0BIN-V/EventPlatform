using DiServiceInstaller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Application.Contracts.Services;
using Notification.Infrastructure.Services;

namespace Notification.Infrastructure.ServiceInstallers;

public class EmailServiceInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEmailService, FluentEmailService>();
    }
}