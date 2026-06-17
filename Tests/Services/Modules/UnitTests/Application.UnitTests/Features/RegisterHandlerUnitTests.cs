using Application.Features.Register;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UnitTests.Features;

public class RegisterHandlerUnitTests
{
    static UserManager<User> CreateUserManager(IUserStore<User>? store = null)
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

        // Return a substitute for UserManager so NSubstitute matchers (Arg.Any, Arg.Is) can be used on its methods
        return For<UserManager<User>>(store, identityOptions, pwdHasher, userValidators, pwdValidators, normalizer,
            describer, services, logger);
    }

    [Fact]
    public async Task Register_Succeeds_ReturnsUserId()
    {
        // Arrange
        var validator = For<IValidator<RegisterRequest>>();
        validator.ValidateAsync(Arg.Any<RegisterRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var userManager = CreateUserManager();

        userManager.FindByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult<User?>(null));
        userManager.CreateAsync(Arg.Any<User>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success));

        var handler = new RegisterHandler(validator, userManager);
        var request = new RegisterRequest("John", "Doe", "john@example.com", "Password123");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT0.ShouldBeTrue(); // string user id
        var userId = result.AsT0;
        userId.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_ValidationFails_ReturnsValidationFailures()
    {
        // Arrange
        var failures = new List<ValidationFailure> { new("Email", "Invalid") };
        var validationResult = new ValidationResult(failures);
        var validator = For<IValidator<RegisterRequest>>();
        validator.ValidateAsync(Arg.Any<RegisterRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        var userManager = CreateUserManager();
        var handler = new RegisterHandler(validator, userManager);
        var request = new RegisterRequest(null, null, "bad", "123");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT1.ShouldBeTrue(); // List<ValidationFailure>
        var list = result.AsT1;
        list.ShouldNotBeEmpty();
        list.First().PropertyName.ShouldBe("Email");
    }

    [Fact]
    public async Task Register_UserAlreadyExists_ReturnsError()
    {
        // Arrange
        var validator = For<IValidator<RegisterRequest>>();
        validator.ValidateAsync(Arg.Any<RegisterRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var existing = new User { Id = "existing-id", Email = "john@example.com" };
        var userStore = For<IUserStore<User>>();
        var userManager = CreateUserManager(userStore);
        userManager.FindByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult<User?>(existing));

        var handler = new RegisterHandler(validator, userManager);
        var request = new RegisterRequest("John", "Doe", "john@example.com", "Password123");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue(); // UserAlreadyExistsError
        var error = result.AsT2;
        error.Email.ShouldBe("john@example.com");
    }

    [Fact]
    public async Task Register_CreateFails_ReturnsValidationFailuresFromIdentity()
    {
        // Arrange
        var validator = For<IValidator<RegisterRequest>>();
        validator.ValidateAsync(Arg.Any<RegisterRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        var userStore = For<IUserStore<User>>();
        var userManager = CreateUserManager(userStore);
        userManager.FindByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult<User?>(null));

        var idErrors = new[] { new IdentityError { Code = "Pwd", Description = "Weak" } };
        var failed = IdentityResult.Failed(idErrors);
        userManager.CreateAsync(Arg.Any<User>(), Arg.Any<string>()).Returns(Task.FromResult(failed));

        var handler = new RegisterHandler(validator, userManager);
        var request = new RegisterRequest("John", "Doe", "john@example.com", "123");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT1.ShouldBeTrue(); // List<ValidationFailure>
        var list = result.AsT1;
        list.ShouldNotBeEmpty();
        list.First().ErrorCode.ShouldBe("Pwd");
    }
}