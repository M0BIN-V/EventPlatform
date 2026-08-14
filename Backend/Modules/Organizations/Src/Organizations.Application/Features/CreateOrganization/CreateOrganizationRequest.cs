using Organizations.Application.Common.Validators;

namespace Organizations.Application.Features.CreateOrganization;

public record CreateOrganizationRequest(string Name, string Slug, string? Description);

public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .IsOrganizationName();

        RuleFor(x => x.Slug)
            .NotEmpty()
            .IsOrganizationSlug();

        RuleFor(x => x.Description)
            .IsOrganizationDescription();
    }
}