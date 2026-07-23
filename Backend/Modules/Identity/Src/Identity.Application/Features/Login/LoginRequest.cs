using Identity.Application.Common.Validations;
using FluentValidation;

namespace Identity.Application.Features.Login;

public record LoginRequest(string Email, string Password);

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .ValidEmail();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
