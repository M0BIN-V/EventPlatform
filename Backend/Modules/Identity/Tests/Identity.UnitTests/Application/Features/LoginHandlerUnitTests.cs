using Application.UnitTests.Application.Common;
using Application.UnitTests.Application.Common.Abstractions;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Common.Options;
using Identity.Application.Features.Login;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace Application.UnitTests.Application.Features;

public class LoginHandlerUnitTests : HandlerTest<LoginHandler, LoginRequest, LoginResponse>
{
    private readonly IAccessTokenService _accessTokenService = For<IAccessTokenService>();
    private readonly IRefreshTokenHasher _hasher = For<IRefreshTokenHasher>();
    private readonly IOptions<RefreshTokenOptions> _options = For<IOptions<RefreshTokenOptions>>();
    private readonly IRefreshTokenRepository _repository = For<IRefreshTokenRepository>();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly ISecureTokenGenerator _tokenGenerator = For<ISecureTokenGenerator>();
    private readonly IIdentityUnitOfWork _uow = For<IIdentityUnitOfWork>();
    private readonly UserManager<User> _userManager = new FakeUserManagerBuilder().Create();

    public LoginHandlerUnitTests()
    {
        var validator = For<IValidator<LoginRequest>>();

        _options.Value.Returns(new RefreshTokenOptions { ExpirationDays = 7 });

        Handler = new LoginHandler(
            _timeProvider,
            validator,
            _tokenGenerator,
            _hasher,
            _repository,
            _uow,
            _options,
            _userManager,
            _accessTokenService);

        validator.ValidateAsync(Any<LoginRequest>(), Any<CancellationToken>())
            .Returns(new ValidationResult());

        Validator = validator;
    }

    protected override LoginHandler Handler { get; }
    protected override IValidator<LoginRequest> Validator { get; }

    protected override LoginRequest GetRequest()
    {
        return new LoginRequest("email@mail.com", "password");
    }

    [Fact]
    public async Task Handler_WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var request = GetRequest();

        _userManager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(null));

        // Act
        var result = await Handler.HandleAsync(request);

        // Assert
        result.Value.ShouldBeOfType<UserNotFoundError>();
    }

    [Fact]
    public async Task Handler_WhenPasswordIsInvalid_ShouldReturnInvalidPasswordError()
    {
        // Arrange
        var request = GetRequest();
        var user = new User { Id = "user-id" };

        _userManager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(user));

        _userManager.CheckPasswordAsync(user, Any<string>())
            .Returns(Task.FromResult(false));

        // Act
        var result = await Handler.HandleAsync(request);

        // Assert
        result.Value.ShouldBeOfType<InvalidPasswordError>();
    }

    [Fact]
    public async Task Handler_WhenCredentialsAreValid_ShouldReturnTokensAndPersistRefreshToken()
    {
        // Arrange
        var request = GetRequest();
        var user = new User { Id = "this-is-the-user-id" };
        const string accessToken = "this-is-the-access-token";
        const string rawRefreshToken = "raw-refresh-token";
        const string refreshTokenHash = "refresh-token-hash";
        user.EmailConfirmed = true;

        _userManager.FindByEmailAsync(Any<string>())
            .Returns(Task.FromResult<User?>(user));

        _userManager.CheckPasswordAsync(user, Any<string>())
            .Returns(Task.FromResult(true));

        _userManager.GetRolesAsync(user)
            .Returns(Task.FromResult<IList<string>>(new List<string> { "Admin" }));

        _accessTokenService.GenerateAccessToken(Any<User>(), Any<List<string>>())
            .Returns(accessToken);

        _tokenGenerator.Generate()
            .Returns(rawRefreshToken);

        _hasher.HashToken(rawRefreshToken)
            .Returns(refreshTokenHash);

        // Act
        var result = await Handler.HandleAsync(request);

        // Assert
        result.Value.ShouldBeOfType<LoginTokenResponse>();

        var response = result.Value.As<LoginTokenResponse>();
        response.AccessToken.ShouldBe(accessToken);
        response.RefreshToken.ShouldBe(rawRefreshToken);
    }
}