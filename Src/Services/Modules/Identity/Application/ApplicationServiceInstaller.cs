using Application.Features.Register;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddIdentityModuleApplication(this IHostApplicationBuilder builder)
    {
        // add application handlers 
        builder.Services.AddScoped<RegisterHandler>();
        
        return builder;
    }
}