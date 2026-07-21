using FluentValidation;
using FluentValidation.Results;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Errors;
using Identity.Application.Features.Refresh;
using Identity.Domain.Entities;
using OneOf;

namespace Application.UnitTests.Features.Authentication;

public class RefreshHandlerTests
{
    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokenPair()
    {
        // Arrange
        var validator = For<IValidator<RefreshRequest>>();
        validator.ValidateAsync(Any<RefreshRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var user = new User { Id = "user-1", Email = "john@example.com", FirstName = "John", LastName = "Doe" };
        var oldToken = new RefreshToken { Id = "rt-1", UserId = "user-1", TokenHash = "old-hash" };

        var refreshTokenService = For<IRefreshTokenService>();
        refreshTokenService.ValidateAsync(Any<string>(), Any<CancellationToken>())
            .Returns(Task.FromResult(
                OneOf<(User, RefreshToken), InvalidRefreshTokenError>.FromT0((user, oldToken))));

        var newToken = new RefreshToken { Id = "rt-2", UserId = "user-1", TokenHash = "new-hash" };
        refreshTokenService.RotateAsync(Any<RefreshToken>(), Any<int>(), Any<string?>(), Any<string?>(),
                Any<CancellationToken>())
            .Returns(Task.FromResult(("new-refresh-token", newToken)));

        var tokenService = For<ITokenService>();
        tokenService.GenerateAccessTokenAsync(Any<User>()).Returns("new-access-token");

        var handler = new RefreshHandler(validator, tokenService, refreshTokenService);
        var request = new RefreshRequest("valid-refresh-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT0.ShouldBeTrue();
        var tokens = result.AsT0;
        tokens.AccessToken.ShouldBe("new-access-token");
        tokens.RefreshToken.ShouldBe("new-refresh-token");

        // Verify rotation was called
        await refreshTokenService.Received(1).RotateAsync(
            oldToken,
            Any<int>(),
            Any<string?>(),
            Any<string?>(),
            Any<CancellationToken>());
    }

    [Fact]
    public async Task Refresh_InvalidToken_ReturnsError()
    {
        // Arrange
        var validator = For<IValidator<RefreshRequest>>();
        validator.ValidateAsync(Any<RefreshRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var error = new InvalidRefreshTokenError("Token is invalid");
        var refreshTokenService = For<IRefreshTokenService>();
        refreshTokenService.ValidateAsync(Any<string>(), Any<CancellationToken>())
            .Returns(Task.FromResult(
                OneOf<(User, RefreshToken), InvalidRefreshTokenError>.FromT1(error)));

        var tokenService = For<ITokenService>();

        var handler = new RefreshHandler(validator, tokenService, refreshTokenService);
        var request = new RefreshRequest("invalid-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue();
        var returnedError = result.AsT2;
        returnedError.Message.ShouldBe("Token is invalid");
    }

    [Fact]
    public async Task Refresh_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new("RefreshToken", "Required") };
        var validationResult = new ValidationResult(failures);
        var validator = For<IValidator<RefreshRequest>>();
        validator.ValidateAsync(Any<RefreshRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        var refreshTokenService = For<IRefreshTokenService>();
        var tokenService = For<ITokenService>();

        var handler = new RefreshHandler(validator, tokenService, refreshTokenService);
        var request = new RefreshRequest("");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT1.ShouldBeTrue();
        var errors = result.AsT1;
        errors.ShouldNotBeEmpty();
    }
}