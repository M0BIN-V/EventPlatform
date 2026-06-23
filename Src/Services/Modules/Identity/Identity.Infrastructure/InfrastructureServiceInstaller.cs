using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Infrastructure;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddIdentityModuleInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<EfIdentityDbContext>("event-platform-db");

        builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<EfIdentityDbContext>()
            .AddDefaultTokenProviders();

        return builder;
    }
}