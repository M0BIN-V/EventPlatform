using DiServiceInstaller;

namespace WebApi.ServiceInstallers;

public class OpenApiInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
    }
}