using BuildingBlocks.Application.Contracts;
using Organizations.Application.Common.Contracts.Persistence;
using Organizations.Application.Common.Validators;

namespace Organizations.Application.Features.ChangeProfilePicture;

public record ChangeProfilePictureRequest(string Slug);

public class ChangeProfilePictureRequestValidator : AbstractValidator<ChangeProfilePictureRequest>
{
    public ChangeProfilePictureRequestValidator()
    {
        RuleFor(x => x.Slug)
            .NotNull()
            .NotEmpty()
            .IsOrganizationSlug();
    }
}

public union ChangeProfilePictureResponse(
    List<ValidationFailure>,
    OrganizationUnauthorizedError,
    OrganizationNotFoundError);

public class ChangeProfilePictureHandler(
    ICurrentUser currentUser,
    IOrganizationRepository orgRepo,
    IValidator<ChangeProfilePictureRequest> validator
) : Handler<ChangeProfilePictureRequest, ChangeProfilePictureResponse>
{
    public override async Task<ChangeProfilePictureResponse> HandleAsync(ChangeProfilePictureRequest request,
        CancellationToken ct = default)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid) return validationResult.Errors;

        var org = await orgRepo.GetBySlugAsync(request.Slug, ct);
        if (org is null) return new OrganizationNotFoundError(request.Slug);

        if (org.CreatorUserId != currentUser.Id) return new OrganizationUnauthorizedError();

        throw new NotImplementedException();
    }
}