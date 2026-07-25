using Identity.Application.Common.Validations;

namespace Identity.Application.Features.Register;

public record RegisterRequest(string? FirstName, string? LastName, string Email, string Password);

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50).WithName("First name");

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithName("Last name");

        RuleFor(x => x.Email)
            .NotEmpty()
            .ValidEmail();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}