using BuildingBlocks.Presentation.Extensions;
using Identity.Application.Features.ConfirmEmail;
using Identity.Application.Features.Register;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Presentation;

public static class IdentityEndpointExtensions
{
    private static async Task<Results<Ok<string>, ValidationProblem, NotFound<string>, BadRequest<string>>>
        ConfirmEmail(
            [FromServices] ConfirmEmailHandler handler,
            [FromQuery] string userId, [FromQuery] string confirmationToken)
    {
        var request = new ConfirmEmailRequest(userId, confirmationToken);

        var result = await handler.HandleAsync(request);

        return result.Match<Results<Ok<string>, ValidationProblem, NotFound<string>, BadRequest<string>>>(
            confirmed => Ok(confirmed),
            failed => BadRequest(failed.Message),
            userNotFoundError => NotFound(userNotFoundError.Message),
            validationFailure => validationFailure.ToValidationProblem());
    }

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

    public static IEndpointRouteBuilder MapIdentityModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var identityGroup = app.MapGroup("/identity").WithTags("Identity");

        identityGroup.MapPost("/register", Register)
            .WithName("RegisterUser")
            .WithSummary("Registers a new user.")
            .WithDescription("Creates a new user account with the provided details.");
        
        identityGroup.MapGet("confirm-email", ConfirmEmail)
            .WithName("ConfirmEmail")
            .WithSummary("Confirms email address.")
            .WithDescription("Confirms email address.");

        return app;
    }
}