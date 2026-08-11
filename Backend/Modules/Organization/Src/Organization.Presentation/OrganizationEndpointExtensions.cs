using Organization.Application.Features.CreateOrganization;
using System.Security.Claims;

namespace Organization.Presentation;

public static class OrganizationEndpointExtensions
{
    private static async Task<Results<Created<CreateOrganizationResponseData>, ValidationProblem, BadRequest<OrganizationSlugAlreadyExistsError>>> CreateOrganization(
        [FromServices] CreateOrganizationHandler handler,
        [FromBody] CreateOrganizationRequest requestBody,
        ClaimsPrincipal user)
    {
        // Extract UserId from JWT claims
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Defensive check - RequireAuthorization() will handle 401 at the endpoint level
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID not found in JWT claims.");

        // Create request with UserId from JWT
        var request = new CreateOrganizationRequest(requestBody.Name, requestBody.Slug, requestBody.Description, userId);

        var result = await handler.HandleAsync(request);

        return result.Match<Results<Created<CreateOrganizationResponseData>, ValidationProblem, BadRequest<OrganizationSlugAlreadyExistsError>>>(
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
            .WithDescription("Creates a new organization and adds the current user as the owner.");

        return app;
    }
}
