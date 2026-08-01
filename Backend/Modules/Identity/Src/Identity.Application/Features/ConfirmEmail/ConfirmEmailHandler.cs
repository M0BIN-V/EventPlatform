using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Identity.Application.Features.ConfirmEmail;

public class ConfirmEmailHandler(
    IValidator<ConfirmEmailRequest> validator,
    UserManager<User> manager) :
    Handler<ConfirmEmailRequest, ConfirmEmailResponse>
{
    public override async Task<ConfirmEmailResponse> HandleAsync(ConfirmEmailRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var user = await manager.FindByEmailAsync(request.Email);

        var tokenFormatIsValid = TryDecodeBase64Url(request.Token, out var decodedToken);

        if (user is null || !tokenFormatIsValid) return new EmailOrConfirmationTokenIsNotValidError();

        var confirmationResult = await manager.ConfirmEmailAsync(user, decodedToken!);

        if (!confirmationResult.Succeeded) return new EmailOrConfirmationTokenIsNotValidError();

        return "Email Confirmed";
    }

    private static bool TryDecodeBase64Url(string value, out string? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(value)) return false;

        try
        {
            var bytes = WebEncoders.Base64UrlDecode(value);
            result = Encoding.UTF8.GetString(bytes);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}