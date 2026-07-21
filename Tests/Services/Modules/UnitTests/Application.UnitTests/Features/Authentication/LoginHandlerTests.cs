using FluentValidation;
using FluentValidation.Results;
using Identity.Application;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Features.Login;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UnitTests.Features.Authentication;

public class LoginHandlerTests
{
    private static IOptions<JwtOptions> CreateJwtOptions()
    {
        var options = For<IOptions<JwtOptions>>();
        options.Value.Returns(new JwtOptions
        {
            Key = "super-secret-key-that-is-long-enough",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        });
        return options;
    }

    private static UserManager<User> CreateUserManager(IUserStore<User>? store = null)
    {
        store ??= For<IUserStore<User>>();
        var identityOptions = For<IOptions<IdentityOptions>>();
        identityOptions.Value.Returns(new IdentityOptions());
        var pwdHasher = For<IPasswordHasher<User>>();
        var userValidators = new List<IUserValidator<User>>();
        var pwdValidators = new List<IPasswordValidator<User>>();
        var normalizer = For<ILookupNormalizer>();
        var describer = For<IdentityErrorDescriber>();
        var services = For<IServiceProvider>();
        var logger = For<ILogger<UserManager<User>>>();

        return For<UserManager<User>>(store, identityOptions, pwdHasher, userValidators, pwdValidators, normalizer,
            describer, services, logger);
    }

    [Fact]
    public async Task Login_Succeeds_ReturnsAccessAndRefreshTokens()
    {
        // Arrange
        var validator = For<IValidator<LoginRequest>>();
        validator.ValidateAsync(Any<LoginRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var userManager = CreateUserManager();
        var user = new User { Id = "user-1", Email = "john@example.com", FirstName = "John", LastName = "Doe" };
        userManager.FindByEmailAsync(Any<string>()).Returns(Task.FromResult<User?>(user));
        userManager.CheckPasswordAsync(Any<User>(), Any<string>()).Returns(Task.FromResult(true));

        var tokenService = For<ITokenService>();
        tokenService.GenerateAccessTokenAsync(Any<User>()).Returns("access-token");

        var refreshTokenService = For<IRefreshTokenService>();
        var refreshToken = new RefreshToken { Id = "rt-1", TokenHash = "hash", UserId = "user-1" };
        refreshTokenService.GenerateAsync(Any<string>(), Any<int>(), Any<string?>(), Any<string?>(), Any<CancellationToken>())
            .Returns(Task.FromResult(("refresh-token", refreshToken)));

        var handler = new LoginHandler(validator, userManager, tokenService, refreshTokenService, CreateJwtOptions());
        var request = new LoginRequest("john@example.com", "Password123");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT0.ShouldBeTrue();
        var tokens = result.AsT0;
        tokens.AccessToken.ShouldBe("access-token");
        tokens.RefreshToken.ShouldBe("refresh-token");
        
        // Verify refresh token was persisted
        await refreshTokenService.Received(1).GenerateAsync(
            "user-1",
            7,
            null,
            null,
            Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_InvalidEmail_ReturnsUserNotFoundError()
    {
        // Arrange
        var validator = For<IValidator<LoginRequest>>();
        validator.ValidateAsync(Any<LoginRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var userManager = CreateUserManager();
        userManager.FindByEmailAsync(Any<string>()).Returns(Task.FromResult<User?>(null));

        var tokenService = For<ITokenService>();
        var refreshTokenService = For<IRefreshTokenService>();

        var handler = new LoginHandler(validator, userManager, tokenService, refreshTokenService, CreateJwtOptions());
        var request = new LoginRequest("notfound@example.com", "Password123");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue();
        var error = result.AsT2;
        error.Message.ShouldContain("notfound@example.com");
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var validator = For<IValidator<LoginRequest>>();
        validator.ValidateAsync(Any<LoginRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var userManager = CreateUserManager();
        var user = new User { Id = "user-1", Email = "john@example.com" };
        userManager.FindByEmailAsync(Any<string>()).Returns(Task.FromResult<User?>(user));
        userManager.CheckPasswordAsync(Any<User>(), Any<string>()).Returns(Task.FromResult(false));

        var tokenService = For<ITokenService>();
        var refreshTokenService = For<IRefreshTokenService>();

        var handler = new LoginHandler(validator, userManager, tokenService, refreshTokenService, CreateJwtOptions());
        var request = new LoginRequest("john@example.com", "WrongPassword");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT3.ShouldBeTrue();
    }

    [Fact]
    public async Task Login_ValidationFails_ReturnsValidationErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new("Email", "Invalid email") };
        var validationResult = new ValidationResult(failures);
        var validator = For<IValidator<LoginRequest>>();
        validator.ValidateAsync(Any<LoginRequest>(), Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        var userManager = CreateUserManager();
        var tokenService = For<ITokenService>();
        var refreshTokenService = For<IRefreshTokenService>();

        var handler = new LoginHandler(validator, userManager, tokenService, refreshTokenService, CreateJwtOptions());
        var request = new LoginRequest("invalid", "");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT1.ShouldBeTrue();
        var errors = result.AsT1;
        errors.ShouldNotBeEmpty();
    }
}
