using BuildingBlocks.Application.Contracts;
using Organization.Application.Common.Contracts.Persistence;
using Organization.Application.Common.Errors;
using Organization.Domain.Constants;

namespace Organization.Application.Features.CreateOrganization;

public class CreateOrganizationHandler(
    IValidator<CreateOrganizationRequest> validator,
    IOrganizationUnitOfWork unitOfWork) :
    Handler<CreateOrganizationRequest, CreateOrganizationResponse>
{
    public override async Task<CreateOrganizationResponse> HandleAsync(CreateOrganizationRequest request, CancellationToken ct = default)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) 
            return validationResult.Errors;

        // Check if slug already exists
        var slugExists = await unitOfWork.Organizations.SlugExistsAsync(request.Slug!, ct);
        if (slugExists)
            return new OrganizationSlugAlreadyExistsError(request.Slug!);

        // Create organization with creator as the owner
        var organization = new Organization.Domain.Entities.Organization
        {
            Name = request.Name!,
            Slug = request.Slug!.ToLowerInvariant(),
            Description = request.Description,
            CreatorUserId = request.UserId
        };

        // Add organization
        await unitOfWork.Organizations.AddAsync(organization, ct);

        // Create owner membership
        var ownerMember = new Organization.Domain.Entities.OrganizationMember
        {
            OrganizationId = organization.Id,
            UserId = request.UserId,
            Role = OrganizationRole.Owner
        };

        await unitOfWork.Members.AddAsync(ownerMember, ct);

        // Save transaction
        await unitOfWork.SaveChangesAsync(ct);

        return new CreateOrganizationResponseData(organization.Id, organization.Name, organization.Slug);
    }
}

