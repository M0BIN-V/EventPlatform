using Organizations.Application.Common.Validators;

namespace Organizations.Application.Features.EditOrganization;

public record EditOrganizationRequest(string Slug, string? NewName, string? NewSlug, string? NewDescription);

public class EditOrganizationRequestValidator : AbstractValidator<EditOrganizationRequest>
{
    public EditOrganizationRequestValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty()
            .IsOrganizationSlug();

        RuleFor(x => x.NewName)
            .IsOrganizationName();

        RuleFor(x => x.NewSlug)
            .IsOrganizationSlug();

        RuleFor(x => x.NewDescription)
            .IsOrganizationDescription();
    }
}