using System.Text;
using BuildingBlocks.Application.Contracts;
using Identity.Application.Common.Mappers;
using Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

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

        var roleResult = await manager.AddToRoleAsync(newUser, Roles.User);

        if (!roleResult.Succeeded)
            throw new Exception(
                $"Failed to assign role '{Roles.User}' to user '{newUser.Email}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

        var fullName = newUser.FirstName + " " + newUser.LastName;
        var confirmationUrl = await GenerateConfirmationUrl(newUser);
        var message = new ConfirmEmailRequestedEvent(fullName, newUser.Email, confirmationUrl);

        await publisher.PublishAsync(message);

        return newUser.Id;
    }

    private async Task<string> GenerateConfirmationUrl(User user)
    {
        var emailConfirmationToken = await manager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

        var confirmationUrl = $"{options.Value.ConfirmationUrl}?email={user.Email}&token={encodedToken}";

        return confirmationUrl;
    }
}