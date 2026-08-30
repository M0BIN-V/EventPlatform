using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Application.Contracts;
using Organizations.Application.Common.Contracts.Persistence;
using Organizations.Domain.Constants;
using Organizations.Domain.Entities;

namespace Organizations.Application.Features.CreateOrganization;

public class CreateOrganizationHandler(
    IOrganizationRepository organizationsRepo,
    IOrganizationMemberRepository membersRepo,
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

        var slugExists = await organizationsRepo.SlugExistsAsync(request.Slug, ct);
        if (slugExists) return new OrganizationSlugAlreadyExistsError(request.Slug);

        var organization = new Organization(
            request.Name,
            request.Slug.ToLowerInvariant(),
            request.Description,
            userId
        );

        await organizationsRepo.AddAsync(organization, ct);

        var ownerMember = new OrganizationMemberShip
        {
            OrganizationId = organization.Id,
            UserId = userId,
            Role = OrganizationRole.Owner
        };

        await membersRepo.AddAsync(ownerMember, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return new CreateOrganizationResponseData(organization.Id, organization.Name, organization.Slug);
    }
}