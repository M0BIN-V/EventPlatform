using BuildingBlocks.Application.Contracts;
using Organizations.Application.Common.Contracts.Persistence;
using Organizations.Domain.Constants;

namespace Organizations.Application.Features.EditOrganization;

public class EditOrganizationHandler(
    IOrganizationRepository organizationsRepo,
    IValidator<EditOrganizationRequest> validator,
    ICurrentUser currentUser,
    IOrganizationMemberRepository memberRepository,
    IOrganizationUnitOfWork unitOfWork) :
    Handler<EditOrganizationRequest, EditOrganizationResponse>
{
    public override async Task<EditOrganizationResponse> HandleAsync(EditOrganizationRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var organization = await organizationsRepo.GetBySlugAsync(request.Slug, ct);
        if (organization is null)
            return new OrganizationNotFoundError(request.Slug);

        var userId = currentUser.Id;
        var userMembership = await memberRepository.GetByOrganizationAndUserAsync(organization.Id, userId, ct);
        if (userMembership?.Role != OrganizationRole.Owner)
            return new OrganizationUnauthorizedError();

        if (!string.IsNullOrEmpty(request.NewSlug) &&
            request.NewSlug != request.Slug &&
            await organizationsRepo.SlugExistsAsync(request.NewSlug, ct))
            return new OrganizationSlugAlreadyExistsError(request.NewSlug);

        if (!string.IsNullOrEmpty(request.NewName))
            organization.Name = request.NewName;

        if (!string.IsNullOrEmpty(request.NewSlug) && request.NewSlug != request.Slug)
            organization.Slug = request.NewSlug.ToLowerInvariant();

        if (request.NewDescription != organization.Description)
            organization.Description = request.NewDescription;

        await unitOfWork.SaveChangesAsync(ct);

        return new ViewEditedOrganization(organization.Name, organization.Slug, organization.Description);
    }
}