using Application.UnitTests.Application.Common;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Common.Options;
using Identity.Application.Services;
using Identity.Domain.Constants;
using JasperFx.Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace Application.UnitTests.Application.Services;

public class RefreshTokenManagerTests
{
    private readonly IRefreshTokenHasher _hasher = For<IRefreshTokenHasher>();

    private readonly RefreshTokenManager _manager;
    private readonly IOptions<RefreshTokenOptions> _options = For<IOptions<RefreshTokenOptions>>();
    private readonly IRefreshTokenRepository _repository = For<IRefreshTokenRepository>();
    private readonly ISecureTokenGenerator _secureTokenGenerator = For<ISecureTokenGenerator>();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly UserManager<User> _userManager = new FakeUserManagerBuilder().Create();

    public RefreshTokenManagerTests()
    {
        _manager = new RefreshTokenManager(_repository, _hasher, _secureTokenGenerator, _userManager, _timeProvider,
            _options);
    }

    [Fact]
    public async Task RevokeAsync_WhenTokenNotFound_ReturnsInvalidRefreshTokenError()
    {
        //Arrange 
        const string rawToken = "this is token";
        const RevocationReason reason = RevocationReason.Logout;

        _repository.GetByTokenHashAsync(Any<string>(), Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(null));

        //Act
        var result = await _manager.RevokeAsync(rawToken, reason, CancellationToken.None);

        //Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task RevokeAsync_WhenTokenIsExpired_ReturnsInvalidRefreshTokenError()
    {
        //Arrange
        const string rawToken = "this is token";
        const RevocationReason reason = RevocationReason.Logout;

        var now = _timeProvider.GetUtcNow();

        var storedRefreshToken = new RefreshToken
        {
            CreatedAt = now.AddDays(-7),
            ExpiresAt = now.AddDays(-2),
            TokenHash = "this is token hash",
            UserId = "this is user id"
        };

        _timeProvider.Advance(10.Days());

        _repository.GetByTokenHashAsync(Any<string>(), Any<CancellationToken>())
            .Returns(storedRefreshToken);

        //Act
        var result = await _manager.RevokeAsync(rawToken, reason, CancellationToken.None);

        //Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task RevokeAsync_WhenTokenIsValid_ShouldRevokeRefreshToken()
    {
        //Arrange
        const string rawToken = "this is token";
        const RevocationReason reason = RevocationReason.Logout;

        var storedRefreshToken = new RefreshToken
        {
            CreatedAt = _timeProvider.GetUtcNow().AddDays(-1),
            ExpiresAt = _timeProvider.GetUtcNow().AddDays(2),
            TokenHash = "this is token hash",
            UserId = "this is user id"
        };
        _repository.GetByTokenHashAsync(Any<string>(), Any<CancellationToken>())
            .Returns(storedRefreshToken);

        //Act
        var result = await _manager.RevokeAsync(rawToken, reason, CancellationToken.None);

        //Assert
        result.ShouldBeNull();
        storedRefreshToken.IsActive(_timeProvider.GetUtcNow()).ShouldBeFalse();
    }
}