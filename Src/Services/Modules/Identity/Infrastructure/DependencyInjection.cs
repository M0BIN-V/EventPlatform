using Application.Contracts.Services;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddNpgsqlDbContext<IdentityDbContext>("event-platform-db");
        services.AddSingleton<IPasswordHasher, AspPasswordHasher>();

        return services;
    }
}