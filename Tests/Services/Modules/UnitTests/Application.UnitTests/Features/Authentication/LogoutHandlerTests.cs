using FluentValidation;
using FluentValidation.Results;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Errors;
using Identity.Application.Features.Logout;
using Identity.Domain.Entities;
using OneOf;

namespace Application.UnitTests.Features.Authentication;

public class LogoutHandlerTests
{
    [Fact]
    public async Task Logout_ValidToken_RevokesAndReturnsSuccess()
    {
        // Arrange
        var validator = For<IValidator<LogoutRequest>>();
        validator.ValidateAsync(Any<LogoutRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var user = new User { Id = "user-1", Email = "john@example.com" };
        var token = new RefreshToken { Id = "rt-1", UserId = "user-1", TokenHash = "hash" };

        var refreshTokenService = For<IRefreshTokenService>();
        refreshTokenService.ValidateAsync(Any<string>(), Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<(User, RefreshToken), InvalidRefreshTokenError>>(
                OneOf<(User, RefreshToken), InvalidRefreshTokenError>.FromT0((user, token))));
        
        refreshTokenService.RevokeAsync(Any<RefreshToken>(), Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var handler = new LogoutHandler(validator, refreshTokenService);
        var request = new LogoutRequest("valid-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT0.ShouldBeTrue();
        var success = result.AsT0;
        success.Message.ShouldBe("Logged out successfully.");

        // Verify token was revoked
        await refreshTokenService.Received(1).RevokeAsync(token, Any<CancellationToken>());
    }

    [Fact]
    public async Task Logout_InvalidToken_StillReturnsSuccess()
    {
        // Arrange - for security, we don't reveal whether token was invalid
        var validator = For<IValidator<LogoutRequest>>();
        validator.ValidateAsync(Any<LogoutRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var error = new InvalidRefreshTokenError("Token is invalid");
        var refreshTokenService = For<IRefreshTokenService>();
        refreshTokenService.ValidateAsync(Any<string>(), Any<CancellationToken>())
            .Returns(Task.FromResult<OneOf<(User, RefreshToken), InvalidRefreshTokenError>>(
                OneOf<(User, RefreshToken), InvalidRefreshTokenError>.FromT1(error)));

        var handler = new LogoutHandler(validator, refreshTokenService);
        var request = new LogoutRequest("invalid-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        // We should still return success to not expose whether token exists
        result.IsT0.ShouldBeTrue();
        var success = result.AsT0;
        success.Message.ShouldBe("Logged out successfully.");

        // Verify revoke was NOT called since token was invalid
        await refreshTokenService.DidNotReceive().RevokeAsync(Any<RefreshToken>(), Any<CancellationToken>());
    }

    [Fact]
    public async Task Logout_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new("RefreshToken", "Required") };
        var validationResult = new ValidationResult(failures);
        var validator = For<IValidator<LogoutRequest>>();
        validator.ValidateAsync(Any<LogoutRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        var refreshTokenService = For<IRefreshTokenService>();

        var handler = new LogoutHandler(validator, refreshTokenService);
        var request = new LogoutRequest("");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT1.ShouldBeTrue();
        var errors = result.AsT1;
        errors.ShouldNotBeEmpty();
    }
}
