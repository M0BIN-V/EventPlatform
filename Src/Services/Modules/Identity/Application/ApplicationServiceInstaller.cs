using Application.Features.Register;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddIdentityModuleApplication(this IHostApplicationBuilder builder)
    {
        // add application handlers 
        builder.Services.AddScoped<RegisterHandler>();
        
        builder.Services.AddValidatorsFromAssembly(typeof(ApplicationServiceInstaller).Assembly);

        return builder;
    }
}