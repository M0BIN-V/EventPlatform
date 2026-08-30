using System.Security.Claims;
using Organizations.Application.Features.CreateOrganization;
using Organizations.Application.Features.EditOrganization;
using ApplicationEditOrganizationRequest = Organizations.Application.Features.EditOrganization.EditOrganizationRequest;

namespace Organizations.Presentation;

public static class OrganizationEndpointExtensions
{
    private static async Task<Results<
            Created<CreateOrganizationResponseData>,
            ValidationProblem,
            Conflict<OrganizationSlugAlreadyExistsError>>>
        CreateOrganization(
            [FromServices] CreateOrganizationHandler handler,
            [FromBody] CreateOrganizationRequest requestBody,
            ClaimsPrincipal user)
    {
        var request = new CreateOrganizationRequest(requestBody.Name, requestBody.Slug, requestBody.Description);

        var result = await handler.HandleAsync(request);

        return result
            .Match<Results<Created<CreateOrganizationResponseData>, ValidationProblem,
                Conflict<OrganizationSlugAlreadyExistsError>>>(
                org => Created($"/api/organizations/{org.Id}", org),
                validationErrors => validationErrors.ToValidationProblem(),
                slugError => Conflict(slugError)
            );
    }

    private static async Task<Results<
            Ok<ViewEditedOrganization>,
            ValidationProblem,
            NotFound<OrganizationNotFoundError>,
            ForbidHttpResult,
            Conflict<OrganizationSlugAlreadyExistsError>>>
        EditOrganization(
            [FromServices] EditOrganizationHandler handler,
            [FromRoute] string slug,
            [FromBody] EditOrganizationRequest requestBody,
            ClaimsPrincipal user)
    {
        var request =
            new ApplicationEditOrganizationRequest(slug, requestBody.NewName, requestBody.NewSlug,
                requestBody.NewDescription);

        var result = await handler.HandleAsync(request);

        return result
            .Match<Results<Ok<ViewEditedOrganization>, ValidationProblem,
                NotFound<OrganizationNotFoundError>, ForbidHttpResult, Conflict<OrganizationSlugAlreadyExistsError>>>(
                edited => Ok(edited),
                validationErrors => validationErrors.ToValidationProblem(),
                notFoundError => NotFound(notFoundError),
                unauthorizedError => Forbid(),
                slugError => Conflict(slugError)
            );
    }

    public static IEndpointRouteBuilder MapOrganizationModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var orgGroup = app.MapGroup("/organizations")
            .WithTags("Organizations")
            .RequireAuthorization();

        orgGroup.MapPost("", CreateOrganization)
            .WithName("CreateOrganization")
            .WithSummary("Creates a new organization.")
            .WithDescription("Creates a new organization and adds the current user as the owner.")
            .RequireAuthorization();

        orgGroup.MapPut("{slug}", EditOrganization)
            .WithName("EditOrganization")
            .WithSummary("Edits an existing organization.")
            .WithDescription("Edits an existing organization. Only the organization owner can edit it.")
            .RequireAuthorization();

        return app;
    }

    public record EditOrganizationRequest(string NewName, string NewSlug, string NewDescription);
}