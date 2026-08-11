namespace Organization.Application.Features.CreateOrganization;


public record CreateOrganizationRequest(string Name, string Slug, string? Description);

public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithName("Name").WithErrorCode(nameof(OrganizationNameRequiredError));

        RuleFor(x => x.Slug)
            .NotEmpty().WithName("Slug").WithErrorCode(nameof(OrganizationSlugRequiredError))
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithName("Slug")
            .WithErrorCode(nameof(InvalidOrganizationSlugError))
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithName("Description");
    }
}