using System.Net;
using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Errors;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

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

        if (user is null) return new UserNotFoundError(request.Email);

        var decodedToken = WebUtility.UrlDecode(request.Token);

        var confirmationResult = await manager.ConfirmEmailAsync(user, decodedToken);

        if (!confirmationResult.Succeeded) return new EmailConfirmationFailedError(confirmationResult.Errors);
        
        return "Email Confirmed";
    }
}