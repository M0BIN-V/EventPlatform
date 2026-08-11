using System.Collections.Generic;

namespace Organizations.Application.Features.CreateOrganization;

public record CreateOrganizationResponseData(Guid Id, string Name, string Slug);

[GenerateOneOf]
public partial class CreateOrganizationResponse : OneOfBase<
    CreateOrganizationResponseData,
    List<ValidationFailure>,
    OrganizationSlugAlreadyExistsError
>;