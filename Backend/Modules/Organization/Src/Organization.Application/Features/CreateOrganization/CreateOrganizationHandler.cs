using BuildingBlocks.Application.Contracts;
using Organization.Application.Common.Contracts.Persistence;
using Organization.Domain.Constants;
using Organization.Domain.Entities;

namespace Organization.Application.Features.CreateOrganization;

public class CreateOrganizationHandler(
    IValidator<CreateOrganizationRequest> validator,
    ICurrentUser currentUser,
    IOrganizationUnitOfWork unitOfWork) :
    Handler<CreateOrganizationRequest, CreateOrganizationResponse>
{
    public override async Task<CreateOrganizationResponse> HandleAsync(CreateOrganizationRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var userId = currentUser.Id;

        var slugExists = await unitOfWork.Organizations.SlugExistsAsync(request.Slug, ct);
        if (slugExists) return new OrganizationSlugAlreadyExistsError(request.Slug);

        var organization = new Domain.Entities.Organization(
            request.Name,
            request.Slug.ToLowerInvariant(),
            request.Description,
            userId
        );

        await unitOfWork.Organizations.AddAsync(organization, ct);

        var ownerMember = new OrganizationMember
        {
            OrganizationId = organization.Id,
            UserId = userId,
            Role = OrganizationRole.Owner
        };

        await unitOfWork.Members.AddAsync(ownerMember, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return new CreateOrganizationResponseData(organization.Id, organization.Name, organization.Slug);
    }
}