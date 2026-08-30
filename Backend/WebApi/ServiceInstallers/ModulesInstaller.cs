using DiServiceInstaller;
using Files.Presentation;
using Identity.Presentation;
using Notification.Infrastructure;
using Organizations.Presentation;

namespace WebApi.ServiceInstallers;

public class ModulesInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder
            .AddIdentityModule()
            .AddNotificationModule()
            .AddOrganizationModule()
            .AddFilesModule();
    }
}