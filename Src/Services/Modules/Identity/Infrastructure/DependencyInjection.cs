using System.Text;
using Application.Contracts.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<EfIdentityDbContext>("event-platform-db");

        var services = builder.Services;

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<EfIdentityDbContext>()
            .AddDefaultTokenProviders();
        
        var jwt = builder.Configuration.GetSection("Jwt");

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwt["Key"] ??
                                                       throw new NullReferenceException(
                                                           "Jwt:Key configuration is missing")))
                    };
            });

        services.AddSingleton<IPasswordHasher, AspPasswordHasher>();

        return services;
    }
}