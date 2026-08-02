using BuildingBlocks.Infrastructure;
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

        // Register services
        builder.Services.AddScoped<IAccessTokenService, AccessTokenService>()
            .AddScoped<IRefreshTokenHasher, RefreshTokenHasher>()
            .AddScoped<ISecureTokenGenerator, SecureTokenGenerator>();

        // Register repositories 
        builder.Services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Configure options
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

        builder.Services.AddScoped<IModuleInitializer, IdentityModuleInitializer>();

        return builder;
    }
}