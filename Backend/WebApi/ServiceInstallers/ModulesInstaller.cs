using DiServiceInstaller;
using Identity.Presentation;
using Notification.Infrastructure;

namespace WebApi.ServiceInstallers;

public class ModulesInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder
            .AddIdentityModule()
            .AddNotificationModule();
    }
}