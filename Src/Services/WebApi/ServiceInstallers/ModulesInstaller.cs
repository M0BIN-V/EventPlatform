using DiServiceInstaller;
using Endpoints;

namespace WebApi.ServiceInstallers;

public class ModulesInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.AddIdentityModule();
    }
}