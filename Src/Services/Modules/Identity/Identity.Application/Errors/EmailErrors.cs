using BuildingBlocks.Application;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Errors;

public sealed record EmailConfirmationFailedError : Error
{
    public IReadOnlyList<string> Errors { get; }

    public EmailConfirmationFailedError(IEnumerable<IdentityError> errors)
        : base(
            "EmailConfirmationFailed",
            "Email confirmation failed")
    {
        Errors = errors
            .Select(x => x.Description)
            .ToList();
    }
}