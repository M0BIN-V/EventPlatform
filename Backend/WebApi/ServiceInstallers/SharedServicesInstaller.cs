using BuildingBlocks.Application.Contracts;
using DiServiceInstaller;
using WebApi.Services;

namespace WebApi.ServiceInstallers;

public class SharedServicesInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
    }
}