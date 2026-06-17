using System.Text;
using Endpoints.Infrastructure.Data;
using Endpoints.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Endpoints.Infrastructure;

public static class DependencyInjectionExtensions
{
    extension(WebApplicationBuilder builder)
    {
        WebApplicationBuilder SetupIdentityServices()
        {
            builder.Services.AddIdentity<User, IdentityRole>(options => { options.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<EfIdentityDbContext>()
                .AddDefaultTokenProviders();

            return builder;
        }

        WebApplicationBuilder SetupJwt()
        {
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
                                                   throw new NullReferenceException(
                                                       "Jwt:Key configuration is missing")))
                    };
                });

            return builder;
        }

        public WebApplicationBuilder InstallIdentityModule()
        {
            builder.AddNpgsqlDbContext<EfIdentityDbContext>("event-platform-db");

            builder.SetupIdentityServices();

            builder.SetupJwt();

            builder.Services.AddAuthorization();

            return builder;
        }
    }
}