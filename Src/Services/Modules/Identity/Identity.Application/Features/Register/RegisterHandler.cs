using System.Text;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using BuildingBlocks.Application.Events;
using FluentValidation;
using Identity.Application.Common.Mappers;
using Identity.Application.Errors;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.Register;

public class EmailConfirmationOptions
{
    public required string ConfirmationUrl { get; init; }
}

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

        if (!result.Succeeded)
            return result.Errors.ToValidationFailure();

        var message = new ConfirmEmailRequestedEvent(newUser.Email, await GenerateConfirmationUrl(newUser));
        await publisher.PublishAsync(message);

        return newUser.Id;
    }

    private async Task<string> GenerateConfirmationUrl(User user)
    {
        var emailConfirmationToken = await manager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

        var confirmationUrl = $"{options.Value.ConfirmationUrl} ?userId={user.Id}&token={encodedToken}";

        return confirmationUrl;
    }
}