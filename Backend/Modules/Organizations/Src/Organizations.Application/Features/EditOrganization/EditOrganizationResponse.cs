namespace Organizations.Application.Features.EditOrganization;

public record ViewEditedOrganization(string Name, string Slug, string? Description);

[GenerateOneOf]
public partial class EditOrganizationResponse : OneOfBase<
    ViewEditedOrganization,
    List<ValidationFailure>,
    OrganizationNotFoundError,
    OrganizationUnauthorizedError,
    OrganizationSlugAlreadyExistsError
>;