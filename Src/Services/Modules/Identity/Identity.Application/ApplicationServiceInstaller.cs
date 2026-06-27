using FluentValidation;
using Identity.Application.Features.Register;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Application;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddIdentityModuleApplication(this IHostApplicationBuilder builder)
    {
        // add application handlers 
        builder.Services.AddScoped<RegisterHandler>();
        
        builder.Services.AddValidatorsFromAssembly(typeof(ApplicationServiceInstaller).Assembly);

        builder.Services.Configure<EmailConfirmationOptions>(builder.Configuration.GetSection("EmailConfirmationOptions"));

        return builder;
    }
}