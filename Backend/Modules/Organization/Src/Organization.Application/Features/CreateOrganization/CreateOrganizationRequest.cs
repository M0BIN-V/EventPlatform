namespace Organization.Application.Features.CreateOrganization;

public record CreateOrganizationRequest(string Name, string Slug, string? Description);

public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Slug)
            .NotEmpty()
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithName("Description");
    }
}