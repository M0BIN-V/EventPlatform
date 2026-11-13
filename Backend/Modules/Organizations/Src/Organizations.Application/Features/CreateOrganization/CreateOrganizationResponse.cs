namespace Organizations.Application.Features.CreateOrganization;

public record CreateOrganizationResponseData(Guid Id, string Name, string Slug);

public union CreateOrganizationResponse(
    CreateOrganizationResponseData,
    List<ValidationFailure>,
    OrganizationSlugAlreadyExistsError);