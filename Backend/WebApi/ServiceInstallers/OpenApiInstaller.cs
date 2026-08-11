using System.Collections.Concurrent;
using DiServiceInstaller;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace WebApi.ServiceInstallers;

public class OpenApiInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); })
            .AddEndpointsApiExplorer();
    }
}

public class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider schemes) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var jwtBearerScheme = await schemes.GetSchemeAsync("Bearer");
        if (jwtBearerScheme is null) return;

        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes = new ConcurrentDictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Enter JWT Bearer token **_only_**"
        };

        document.Security?.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer")
                {
                    Reference = new OpenApiReferenceWithDescription
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                []
            }
        });
    }
}