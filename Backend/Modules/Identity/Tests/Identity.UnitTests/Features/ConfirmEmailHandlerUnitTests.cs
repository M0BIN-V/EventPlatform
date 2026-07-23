using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Identity.Application.Features.ConfirmEmail;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UnitTests.Features;

public class ConfirmEmailHandlerUnitTests
{
    private static UserManager<User> CreateUserManager()
    {
        var store = For<IUserStore<User>>();
        var options = For<IOptions<IdentityOptions>>();
        options.Value.Returns(new IdentityOptions());

        return For<UserManager<User>>(
            store,
            options,
            For<IPasswordHasher<User>>(),
            new List<IUserValidator<User>>(),
            new List<IPasswordValidator<User>>(),
            For<ILookupNormalizer>(),
            For<IdentityErrorDescriber>(),
            For<IServiceProvider>(),
            For<ILogger<UserManager<User>>>());
    }


    [Fact]
    public async Task ConfirmEmail_InvalidEmail_ReturnsValidationErrors()
    {
        // Arrange
        var validator = For<IValidator<ConfirmEmailRequest>>();

        var failures = new List<ValidationFailure>
        {
            new("Email", "Invalid email format")
        };

        validator.ValidateAsync(
                Any<ConfirmEmailRequest>(),
                Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult(failures)));


        var manager = CreateUserManager();

        var handler = new ConfirmEmailHandler(
            validator,
            manager);


        var request = new ConfirmEmailRequest(
            "invalid-email",
            "token");


        // Act
        var result = await handler.HandleAsync(request);


        // Assert
        result.IsT3.ShouldBeTrue();

        var errors = result.AsT3;

        errors.ShouldNotBeEmpty();
        errors.First().PropertyName.ShouldBe("Email");
    }


    [Fact]
    public async Task ConfirmEmail_UserDoesNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var validator = For<IValidator<ConfirmEmailRequest>>();

        validator.ValidateAsync(
                Any<ConfirmEmailRequest>(),
                Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));


        var manager = CreateUserManager();

        manager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(null));


        var handler = new ConfirmEmailHandler(
            validator,
            manager);


        var request = new ConfirmEmailRequest(
            "john@example.com",
            "token");


        // Act
        var result = await handler.HandleAsync(request);


        // Assert
        result.IsT2.ShouldBeTrue();

        var error = result.AsT2;

        error.Email.ShouldBe("john@example.com");
    }


    [Fact]
    public async Task ConfirmEmail_InvalidToken_ReturnsConfirmationFailedError()
    {
        // Arrange
        var validator = For<IValidator<ConfirmEmailRequest>>();

        validator.ValidateAsync(
                Any<ConfirmEmailRequest>(),
                Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));


        var manager = CreateUserManager();

        var user = new User
        {
            Id = "user-id",
            Email = "john@example.com"
        };


        manager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(user));


        var identityErrors = new[]
        {
            new IdentityError
            {
                Code = "InvalidToken",
                Description = "Invalid email confirmation token"
            }
        };


        manager.ConfirmEmailAsync(
                Any<User>(),
                Any<string>())
            .Returns(Task.FromResult(
                IdentityResult.Failed(identityErrors)));


        var handler = new ConfirmEmailHandler(
            validator,
            manager);


        var token = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes("wrong-token"));


        var request = new ConfirmEmailRequest(
            "john@example.com",
            token);


        // Act
        var result = await handler.HandleAsync(request);


        // Assert
        result.IsT1.ShouldBeTrue();

        var error = result.AsT1;

        error.Errors.ShouldContain(x => x == "Invalid email confirmation token");
    }


    [Fact]
    public async Task ConfirmEmail_ValidToken_ReturnsSuccessMessage()
    {
        // Arrange
        var validator = For<IValidator<ConfirmEmailRequest>>();

        validator.ValidateAsync(
                Any<ConfirmEmailRequest>(),
                Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));


        var manager = CreateUserManager();


        var user = new User
        {
            Id = "user-id",
            Email = "john@example.com"
        };


        manager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(user));


        manager.ConfirmEmailAsync(
                Any<User>(),
                Any<string>())
            .Returns(Task.FromResult(
                IdentityResult.Success));


        var handler = new ConfirmEmailHandler(
            validator,
            manager);


        var token = WebEncoders.Base64UrlEncode("valid-token"u8.ToArray());


        var request = new ConfirmEmailRequest(
            "john@example.com",
            token);


        // Act
        var result = await handler.HandleAsync(request);


        // Assert
        result.IsT0.ShouldBeTrue();

        result.AsT0.ShouldBe("Email Confirmed");
    }
}