using Application.UnitTests.Application.Common;
using Identity.Application.Common.Contracts.ApplicationServices;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Features.Refresh;
using Microsoft.Extensions.Time.Testing;

namespace Application.UnitTests.Application.Features;

public class RefreshHandlerUnitTests
{
    private readonly IAccessTokenService _accessTokenService = For<IAccessTokenService>();
    private readonly RefreshHandler _handler;
    private readonly IRefreshTokenManager _refreshTokenManager = For<IRefreshTokenManager>();
    private readonly UserManager<User> _userManager = new FakeUserManagerBuilder().Create();
    private readonly RefreshRequestValidator _validator = new();

    public RefreshHandlerUnitTests()
    {
        _handler = new RefreshHandler(
            _refreshTokenManager,
            _validator,
            _userManager,
            _accessTokenService,
            For<IIdentityUnitOfWork>());
    }

    [Fact]
    public async Task Handler_WhenRefreshTokenIsEmpty_ShouldReturnValidationError()
    {
        //Arrange 
        var request = new RefreshRequest("");

        //Act
        var result = await _handler.HandleAsync(request);

        //Assert
        result.Value.ShouldBeOfType<List<ValidationFailure>>();
    }

    [Fact]
    public async Task Handler_WhenRefreshTokenIsValid_ShouldReturnNewTokens()
    {
        //Arrange 
        var request = new RefreshRequest("valid-refresh-token");
        var timeProvider = new FakeTimeProvider();
        const string refreshTokenHash = "this-is-the-hash";
        const string accessToken = "this-is-the-access-token";
        const string newRefreshToken = "new-refresh-token";
        _accessTokenService.GenerateAccessToken(Any<User>(), Any<List<string>>())
            .Returns(accessToken);

        var managerResult = (
            newRefreshToken,
            new RefreshToken
            {
                CreatedAt = timeProvider.GetUtcNow(),
                TokenHash = refreshTokenHash,
                UserId = "this-is-the-user-id",
                ExpiresAt = timeProvider.GetUtcNow().AddDays(7)
            },
            new User());

        _refreshTokenManager.RotateAsync(Any<string>(), Any<CancellationToken>())
            .Returns(managerResult);

        //Act
        var result = await _handler.HandleAsync(request);

        //Assert
        result.Value.ShouldBeOfType<RefreshTokenResponse>();

        var response = result.Value.As<RefreshTokenResponse>();
        response.RefreshToken.ShouldBe(newRefreshToken);
        response.AccessToken.ShouldBe(accessToken);
    }

    [Fact]
    public async Task Handler_WhenRefreshTokenIsAlreadyRotated_ShouldReturnInvalidTokenError()
    {
        //Arrange 
        var request = new RefreshRequest("rotated-refresh-token");
        _refreshTokenManager.RotateAsync(Any<string>(), Any<CancellationToken>())
            .Returns(new TokenAlreadyRotatedError());

        //Act
        var result = await _handler.HandleAsync(request);

        //Assert
        result.Value.ShouldBeOfType<InvalidRefreshTokenError>();
    }

    [Fact]
    public async Task Handler_WhenUserNotFound_ShouldReturnInvalidTokenError()
    {
        //Arrange 
        var request = new RefreshRequest("rotated-refresh-token");
        _refreshTokenManager.RotateAsync(Any<string>(), Any<CancellationToken>())
            .Returns(new UserNotFoundError("user@email.em"));

        //Act
        var result = await _handler.HandleAsync(request);

        //Assert
        result.Value.ShouldBeOfType<InvalidRefreshTokenError>();
    }
}