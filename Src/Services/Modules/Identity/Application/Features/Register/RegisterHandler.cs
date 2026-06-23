using System.Text;
using Application.Errors;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using BuildingBlocks.Application.Events;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Application.Features.Register;

public record EmailConfirmationOptions(string FrontendConfirmationUrl);

public class RegisterHandler(
    IMessagePublisher publisher,
    IValidator<RegisterRequest> validator,
    IOptions<EmailConfirmationOptions> options,
    UserManager<User> manager) :
    Handler<RegisterRequest, RegisterResponse>
{
    public override async Task<RegisterResponse> HandleAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var existingUser = await manager.FindByEmailAsync(request.Email);
        if (existingUser is not null) return new UserAlreadyExistsError(request.Email);

        var newUser = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = Guid.CreateVersion7().ToString()
        };

        var result = await manager.CreateAsync(newUser, request.Password);
        if (result.Succeeded) return newUser.Id;

        var message = new ConfirmEmailRequestedEvent(newUser.Email, await GenerateConfirmationUrl(newUser));
        await publisher.PublishAsync(message);

        // Map IdentityError to FluentValidation.ValidationFailure and set ErrorCode so callers can inspect it
        return result.Errors
            .Select(e =>
                new ValidationFailure(e.Code, e.Description)
                {
                    ErrorCode = e.Code
                })
            .ToList();
    }

    async Task<string> GenerateConfirmationUrl(User user)
    {
        var emailConfirmationToken = await manager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

        var confirmationUrl = $"{options.Value.FrontendConfirmationUrl} ?userId={user.Id}&token={encodedToken}";

        return confirmationUrl;
    }
}