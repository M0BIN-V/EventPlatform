namespace Organizations.Application.Features.EditOrganization;

public record ViewEditedOrganization(string Name, string Slug, string? Description);

public union EditOrganizationResponse(
    ViewEditedOrganization,
    List<ValidationFailure>,
    OrganizationNotFoundError,
    OrganizationUnauthorizedError,
    OrganizationSlugAlreadyExistsError);