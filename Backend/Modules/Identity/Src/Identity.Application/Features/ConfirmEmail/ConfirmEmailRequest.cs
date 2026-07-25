using Identity.Application.Common.Validations;

namespace Identity.Application.Features.ConfirmEmail;

public record ConfirmEmailRequest(string Email, string Token);

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .ValidEmail();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}