using BuildingBlocks.Presentation.Extensions;
using Identity.Application.Features.Register;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Presentation;

public static class IdentityEndpointExtensions
{
    private static async Task<Results<Created, Conflict<string>, ValidationProblem>> Register(
        [FromServices] RegisterHandler handler,
        [FromBody] RegisterRequest request)
    {
        var result = await handler.HandleAsync(request);

        return result.Match<Results<Created, Conflict<string>, ValidationProblem>>(
            userId => Created(),
            validationErrors => validationErrors.ToValidationProblem(),
            userAlreadyExistsError => Conflict(userAlreadyExistsError.Message)
        );
    }

    public static WebApplication MapIdentityModuleEndpoints(this WebApplication app)
    {
        var identityGroup = app.MapGroup("/identity").WithTags("Identity");

        identityGroup.MapPost("/register", Register)
            .WithName("RegisterUser")
            .WithSummary("Registers a new user.")
            .WithDescription("Creates a new user account with the provided details.");

        return app;
    }
}