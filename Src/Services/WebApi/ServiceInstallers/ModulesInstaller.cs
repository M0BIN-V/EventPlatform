using DiServiceInstaller;
using Identity.Presentation;

namespace WebApi.ServiceInstallers;

public class ModulesInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.AddIdentityModule();
    }
}