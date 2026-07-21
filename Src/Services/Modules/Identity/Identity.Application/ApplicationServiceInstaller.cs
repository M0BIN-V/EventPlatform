using BuildingBlocks.Application.Extensions;
using FluentValidation;
using Identity.Application.Features.Login;
using Identity.Application.Features.Register;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Application;

public class JwtOptions
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
}

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddIdentityModuleApplication(this IHostApplicationBuilder builder)
    {
        var assembly = typeof(ApplicationServiceInstaller).Assembly;

        builder.Services.RegisterHandlers(assembly);

        builder.Services.AddValidatorsFromAssembly(assembly);

        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("JwtOptions"));

        builder.Services.Configure<EmailConfirmationOptions>(
            builder.Configuration.GetSection("EmailConfirmationOptions"));

        return builder;
    }
}