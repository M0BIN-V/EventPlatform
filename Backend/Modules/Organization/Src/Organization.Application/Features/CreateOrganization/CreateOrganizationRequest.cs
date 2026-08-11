using FluentValidation.Results;
using Organization.Application.Common.Errors;
using OneOf;

namespace Organization.Application.Features.CreateOrganization;

/// <summary>
/// Internal request object that includes the authenticated user's ID.
/// The UserId is always provided by the endpoint from JWT claims.
/// </summary>
public record CreateOrganizationRequest(string? Name, string? Slug, string? Description, string UserId = "");

public record CreateOrganizationResponseData(Guid Id, string Name, string Slug);

[GenerateOneOf]
public partial class CreateOrganizationResponse : OneOfBase<
    CreateOrganizationResponseData,
    List<ValidationFailure>,
    OrganizationSlugAlreadyExistsError
>;
