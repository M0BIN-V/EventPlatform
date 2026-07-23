using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.DbContext;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Infrastructure;

public static class ApplicationServiceInstaller
{
    public static IHostApplicationBuilder AddIdentityModuleInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<EfIdentityDbContext>(
            "event-platform-db",
            null,
            dbContextOpt => dbContextOpt.UseNpgsql(npgOpt => npgOpt
                .MigrationsHistoryTable("__EFMigrationsHistory", EfIdentityDbContext.Schema)));

        builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<EfIdentityDbContext>()
            .AddDefaultTokenProviders();

        // Register token and refresh token services
        builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
        builder.Services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
        builder.Services.AddScoped<ISecureTokenGenerator, SecureTokenGenerator>();

        // Register repositories 
        builder.Services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("Jwt"));

        return builder;
    }
}