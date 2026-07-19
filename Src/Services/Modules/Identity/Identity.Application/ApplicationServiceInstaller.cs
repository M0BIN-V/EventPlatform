using BuildingBlocks.Application.Extensions;
using FluentValidation;
using Identity.Application.Features.Register;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Application;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddIdentityModuleApplication(this IHostApplicationBuilder builder)
    {
        var assembly = typeof(ApplicationServiceInstaller).Assembly;

        builder.Services.RegisterHandlers(assembly);

        builder.Services.AddValidatorsFromAssembly(assembly);

        builder.Services.Configure<EmailConfirmationOptions>(
            builder.Configuration.GetSection("EmailConfirmationOptions"));

        return builder;
    }
}