using System.Text;
using Endpoints.Data;
using Endpoints.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Endpoints;

public static class DependencyInjectionExtensions
{
    public static WebApplicationBuilder InstallIdentityModule(this WebApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<EfIdentityDbContext>("event-platform-db");

        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<EfIdentityDbContext>()
            .AddDefaultTokenProviders();

        var jwt = builder.Configuration.GetSection("Jwt");

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt["Key"] ??
                                               throw new NullReferenceException("Jwt:Key configuration is missing")))
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}