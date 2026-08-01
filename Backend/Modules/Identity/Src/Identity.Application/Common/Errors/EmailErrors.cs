using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Common.Errors;

public record EmailOrConfirmationTokenIsNotValidError()
    : Error(nameof(EmailOrConfirmationTokenIsNotValidError), "Email or confirmation token is not valid");

public record EmailNotConfirmedError() : Error(nameof(EmailNotConfirmedError), "Email not confirmed");

public sealed record EmailConfirmationFailedError : Error
{
    public EmailConfirmationFailedError(IEnumerable<IdentityError> errors)
        : base(
            "EmailConfirmationFailed",
            "Email confirmation failed")
    {
        Errors = errors
            .Select(x => x.Description)
            .ToList();
    }

    public IReadOnlyList<string> Errors { get; }
}