using System.Text;
using DiServiceInstaller;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.ServiceInstallers;

public class AuthInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        var jwt = builder.Configuration.GetSection("Jwt");

        builder.Services.AddAuthorization();

        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];
        var key = jwt["Key"];

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key ?? throw new NullReferenceException("Jwt:Key configuration is missing")))
        };

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
                options.TokenValidationParameters = validationParameters;

                if (!builder.Environment.IsDevelopment()) return;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine("JWT OnMessageReceived");
                        Console.WriteLine(
                            $"Token: {context.Request.Headers.Authorization}");

                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("JWT AuthenticationFailed");
                        Console.WriteLine(context.Exception);

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("JWT TokenValidated");

                        return Task.CompletedTask;
                    }
                };
            });
    }
}