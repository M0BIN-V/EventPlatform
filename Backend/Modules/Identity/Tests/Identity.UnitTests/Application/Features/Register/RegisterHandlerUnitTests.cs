using Application.UnitTests.Application.Common;
using Application.UnitTests.Application.Common.Abstractions;
using BuildingBlocks.Application.Contracts;
using Identity.Application.Features.Register;
using Messaging;
using Microsoft.Extensions.Options;

namespace Application.UnitTests.Application.Features.Register;

public class RegisterHandlerUnitTests :
    HandlerTest<RegisterHandler, RegisterRequest, RegisterResponse>
{
    private readonly IOptions<EmailConfirmationOptions> _emailConfirmationOptions =
        For<IOptions<EmailConfirmationOptions>>();

    private readonly IMessagePublisher _publisher = For<IMessagePublisher>();
    private readonly UserManager<User> _userManager;
    private readonly FakeUserManagerBuilder _userManagerBuilder = new();

    public RegisterHandlerUnitTests()
    {
        var validator = For<IValidator<RegisterRequest>>();

        _userManager = _userManagerBuilder.Create();

        _emailConfirmationOptions.Value.Returns(new EmailConfirmationOptions { ConfirmationUrl = "frontend-url" });

        Handler = new RegisterHandler(
            _publisher,
            validator,
            _emailConfirmationOptions,
            _userManager);
        validator.ValidateAsync(Any<RegisterRequest>(), Any<CancellationToken>())
            .Returns(new ValidationResult());

        Validator = validator;
    }

    protected override RegisterHandler Handler { get; }
    protected override IValidator<RegisterRequest> Validator { get; }


    protected override RegisterRequest GetRequest()
    {
        return new RegisterRequest("John", "Doe", "email", "password");
    }


    [Fact]
    public async Task Handler_WhenEmailAlreadyExists_ShouldReturnUserAlreadyExistsError()
    {
        //Arrange 
        var request = new RegisterRequest("John", "Doe", "user@email.mail", "password");

        _userManager.FindByEmailAsync(Any<string>())!
            .Returns(new User());

        //Act
        var result = await Handler.HandleAsync(request);

        //Assert
        result.Value.ShouldBeOfType<UserAlreadyExistsError>();
    }

    [Fact]
    public async Task Handler_WhenCreationFailed_ShouldReturnValidationError()
    {
        //Arrange 
        var request = new RegisterRequest("John", "Doe", "email", "password");

        var identityResult = IdentityResult.Failed(new IdentityError
            { Code = "this is error code ", Description = "this is error description" });

        _userManager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(null));

        _userManager.CreateAsync(Any<User>(), Any<string>())
            .Returns(identityResult);

        //Act 
        var result = await Handler.HandleAsync(request);

        //Assert 
        result.Value.ShouldBeOfType<List<ValidationFailure>>();
    }

    [Fact]
    public async Task Handler_WhenRegistrationSucceeded_ShouldReturnSuccessAndPublishConfirmationEvent()
    {
        //Arrange 
        var request = new RegisterRequest("John", "Doe", "email", "password");

        _userManager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(null));

        _userManager.CreateAsync(Any<User>(), Any<string>())
            .Returns(IdentityResult.Success);

        _userManager.AddToRoleAsync(Any<User>(), Any<string>())
            .Returns(IdentityResult.Success);

        //Act
        var result = await Handler.HandleAsync(request);

        //Assert
        result.Value.ShouldBeOfType<string>();
        _ = _publisher.Received(1).PublishAsync(Any<ConfirmEmailRequestedEvent>());
    }
}