using Application.UnitTests.Application.Common;
using Identity.Application.Features.ConfirmEmail;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.UnitTests.Application.Features;

public class ConfirmEmailHandlerUnitTests
{
    private readonly ConfirmEmailHandler _handler;
    private readonly UserManager<User> _userManager;
    private readonly FakeUserManagerBuilder _userManagerBuilder = new();
    private readonly IValidator<ConfirmEmailRequest> _validator = For<IValidator<ConfirmEmailRequest>>();

    public ConfirmEmailHandlerUnitTests()
    {
        _userManager = _userManagerBuilder.Create();

        _validator.ValidateAsync(Any<ConfirmEmailRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        _handler = new ConfirmEmailHandler(_validator, _userManager);
    }

    [Fact]
    public async Task ConfirmEmail_WhenUserDoesNotExist_ReturnsEmailOrConfirmationTokenIsNotValidError()
    {
        // Arrange
        _userManager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(null));

        var request = new ConfirmEmailRequest(
            "john@example.com",
            "token");


        // Act
        var result = await _handler.HandleAsync(request);


        // Assert
        result.Value.ShouldBeOfType<EmailOrConfirmationTokenIsNotValidError>();
    }


    [Fact]
    public async Task ConfirmEmail_WithInvalidToken_ReturnsEmailOrConfirmationTokenIsNotValidError()
    {
        // Arrange
        var user = new User
        {
            Id = "user-id",
            Email = "john@example.com"
        };

        _userManager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(user));

        var identityErrors = new[]
        {
            new IdentityError
            {
                Code = "InvalidToken",
                Description = "Invalid email confirmation token"
            }
        };

        _userManager.ConfirmEmailAsync(Any<User>(), Any<string>())
            .Returns(Task.FromResult(IdentityResult.Failed(identityErrors)));

        var token = WebEncoders.Base64UrlEncode("wrong-token"u8.ToArray());

        var request = new ConfirmEmailRequest("john@example.com", token);


        // Act
        var result = await _handler.HandleAsync(request);


        // Assert
        result.Value.ShouldBeOfType<EmailOrConfirmationTokenIsNotValidError>();
    }


    [Fact]
    public async Task ConfirmEmail_WithValidToken_ReturnsSuccessMessage()
    {
        // Arrange
        var user = new User
        {
            Id = "user-id",
            Email = "john@example.com"
        };

        _userManager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(user));

        _userManager.ConfirmEmailAsync(Any<User>(), Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success));

        var token = WebEncoders.Base64UrlEncode("valid-token"u8.ToArray());

        var request = new ConfirmEmailRequest(user.Email, token);


        // Act
        var result = await _handler.HandleAsync(request);


        // Assert
        result.Value
            .ShouldBeOfType<string>()
            .ShouldBe("Email Confirmed");
    }
}