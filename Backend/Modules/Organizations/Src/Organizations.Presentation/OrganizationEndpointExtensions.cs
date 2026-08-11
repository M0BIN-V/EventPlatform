using System.Security.Claims;
using Organizations.Application.Features.CreateOrganization;

namespace Organization.Presentation;

public static class OrganizationEndpointExtensions
{
    private static async Task<Results<
            Created<CreateOrganizationResponseData>,
            ValidationProblem,
            BadRequest<OrganizationSlugAlreadyExistsError>>>
        CreateOrganization(
            [FromServices] CreateOrganizationHandler handler,
            [FromBody] CreateOrganizationRequest requestBody,
            ClaimsPrincipal user)
    {
        var request = new CreateOrganizationRequest(requestBody.Name, requestBody.Slug, requestBody.Description);

        var result = await handler.HandleAsync(request);

        return result
            .Match<Results<Created<CreateOrganizationResponseData>, ValidationProblem,
                BadRequest<OrganizationSlugAlreadyExistsError>>>(
                org => Created($"/api/organizations/{org.Id}", org),
                validationErrors => validationErrors.ToValidationProblem(),
                slugError => BadRequest(slugError)
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

        return app;
    }
}