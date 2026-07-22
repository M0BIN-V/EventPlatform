using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Common.Options;
using Identity.Application.Errors;
using Identity.Application.Features.Refresh;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UnitTests.Features;

public class RefreshHandlerUnitTests
{
    private static IOptions<RefreshTokenOptions> CreateRefreshTokenOptions(int expirationDays = 7)
    {
        var options = For<IOptions<RefreshTokenOptions>>();
        options.Value.Returns(new RefreshTokenOptions { ExpirationDays = expirationDays });
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

    private static IRefreshTokenRepository CreateRefreshTokenRepository()
    {
        return For<IRefreshTokenRepository>();
    }

    private static ISecureTokenGenerator CreateSecureTokenGenerator(
        string generatedToken = "generated-secure-token-12345")
    {
        var generator = For<ISecureTokenGenerator>();
        generator.Generate().Returns(generatedToken);
        return generator;
    }

    private static IRefreshTokenHasher CreateRefreshTokenHasher()
    {
        var hasher = For<IRefreshTokenHasher>();
        // Create a consistent hasher that returns the same hash for the same input
        var hashMap = new Dictionary<string, string>();
        hasher.HashToken(Any<string>()).Returns(x =>
        {
            var token = (string?)x[0];
            if (token == null) return "hash-of-null";
            if (!hashMap.ContainsKey(token)) hashMap[token] = $"hash-of-{token}";
            return hashMap[token];
        });
        return hasher;
    }

    private static IAccessTokenService CreateAccessTokenService(
        string generatedAccessToken = "generated-access-token-12345")
    {
        var service = For<IAccessTokenService>();
        service.GenerateAccessToken(Any<User>(), Any<List<string>>())
            .Returns(generatedAccessToken);
        return service;
    }

    private static IIdentityUnitOfWork CreateIdentityUnitOfWork()
    {
        var uow = For<IIdentityUnitOfWork>();
        uow.SaveChangesAsync(Any<CancellationToken>()).Returns(Task.CompletedTask);
        return uow;
    }

    [Fact]
    public async Task Refresh_EmptyRefreshToken_ReturnsValidationErrors()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var secureTokenGenerator = CreateSecureTokenGenerator();
        var hasher = CreateRefreshTokenHasher();
        var accessTokenService = CreateAccessTokenService();
        var uow = CreateIdentityUnitOfWork();

        var handler = new RefreshHandler(
            validator,
            CreateUserManager(),
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest("");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT1.ShouldBeTrue(); // List<ValidationFailure>
        var validationErrors = result.AsT1;
        validationErrors.ShouldNotBeEmpty();
        validationErrors.First().PropertyName.ShouldBe(nameof(RefreshRequest.RefreshToken));
        validationErrors.First().ErrorMessage.ShouldContain("required");
    }

    [Fact]
    public async Task Refresh_NullRefreshToken_ReturnsValidationErrors()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var secureTokenGenerator = CreateSecureTokenGenerator();
        var hasher = CreateRefreshTokenHasher();
        var accessTokenService = CreateAccessTokenService();
        var uow = CreateIdentityUnitOfWork();

        var handler = new RefreshHandler(
            validator,
            CreateUserManager(),
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest(null!);

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT1.ShouldBeTrue(); // List<ValidationFailure>
        var validationErrors = result.AsT1;
        validationErrors.ShouldNotBeEmpty();
        validationErrors.First().PropertyName.ShouldBe(nameof(RefreshRequest.RefreshToken));
    }

    [Fact]
    public async Task Refresh_ValidRefreshToken_TokenNotInDatabase_ReturnsInvalidRefreshTokenError()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        repository.GetByTokenHashAsync(Any<string>(), Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(null));

        var hasher = CreateRefreshTokenHasher();
        var secureTokenGenerator = CreateSecureTokenGenerator();
        var accessTokenService = CreateAccessTokenService();
        var uow = CreateIdentityUnitOfWork();

        var handler = new RefreshHandler(
            validator,
            CreateUserManager(),
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest("valid-refresh-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue(); // InvalidRefreshTokenError
        var error = result.AsT2;
        error.ShouldBeOfType<InvalidRefreshTokenError>();
    }

    [Fact]
    public async Task Refresh_RotatedToken_RevokesAllActiveTokensAndReturnsError()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var hasher = CreateRefreshTokenHasher();
        var secureTokenGenerator = CreateSecureTokenGenerator();
        var accessTokenService = CreateAccessTokenService();
        var uow = CreateIdentityUnitOfWork();

        const string userId = "user-123";
        const string incomingToken = "rotated-token";

        // Create a new token that will replace the old one
        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hasher.HashToken("new-token"),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        // Create the rotated token and use Rotate() to properly set the replacement
        var rotatedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hasher.HashToken(incomingToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        rotatedToken.Rotate(newToken);

        var tokenHash = hasher.HashToken(incomingToken);
        repository.GetByTokenHashAsync(tokenHash, Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(rotatedToken));

        var activeTokens = new List<RefreshToken>
        {
            rotatedToken,
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = hasher.HashToken("active-token-1"),
                ExpiresAt = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = hasher.HashToken("active-token-2"),
                ExpiresAt = DateTime.UtcNow.AddDays(3)
            }
        };

        repository.GetActiveTokensAsync(userId, Any<CancellationToken>())
            .Returns(Task.FromResult(activeTokens));

        var handler = new RefreshHandler(
            validator,
            CreateUserManager(),
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest(incomingToken);

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue(); // InvalidRefreshTokenError

        // Verify that all active tokens were revoked
        foreach (var token in activeTokens) token.IsRevoked.ShouldBeTrue();

        // Verify repository.Update was called with active tokens
        repository.Received(1).Update(Is<List<RefreshToken>>(x => x != null && x.Count == 3));

        // Verify UoW SaveChanges was called
        await uow.Received(1).SaveChangesAsync(Any<CancellationToken>());
    }

    [Fact]
    public async Task Refresh_TokenHashMismatch_ReturnsInvalidRefreshTokenError()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var hasher = CreateRefreshTokenHasher();
        var secureTokenGenerator = CreateSecureTokenGenerator();
        var accessTokenService = CreateAccessTokenService();
        var uow = CreateIdentityUnitOfWork();

        const string userId = "user-123";
        const string storedTokenHash = "hash-of-stored-token";

        var storedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = storedTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        // Setup repository to return the token when queried with incoming hash
        // But the token's stored hash won't match
        repository.GetByTokenHashAsync(Any<string>(), Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(storedToken));

        var handler = new RefreshHandler(
            validator,
            CreateUserManager(),
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest("incoming-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue(); // InvalidRefreshTokenError
    }

    [Fact]
    public async Task Refresh_UserNotFound_ReturnsInvalidRefreshTokenError()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var hasher = CreateRefreshTokenHasher();
        var secureTokenGenerator = CreateSecureTokenGenerator();
        var accessTokenService = CreateAccessTokenService();
        var uow = CreateIdentityUnitOfWork();

        const string userId = "user-123";
        const string tokenHash = "hash-of-valid-token";

        var storedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        repository.GetByTokenHashAsync(tokenHash, Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(storedToken));

        var userManager = CreateUserManager();
        userManager.FindByIdAsync(userId).Returns(Task.FromResult<User?>(null));

        var handler = new RefreshHandler(
            validator,
            userManager,
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest("valid-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue(); // InvalidRefreshTokenError
    }

    [Fact]
    public async Task Refresh_ValidActiveToken_GeneratesNewTokensAndReturnsSuccess()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var hasher = CreateRefreshTokenHasher();
        const string newRawToken = "new-generated-raw-token";
        var secureTokenGenerator = CreateSecureTokenGenerator(newRawToken);
        const string newAccessToken = "new-access-token-jwt";
        var accessTokenService = CreateAccessTokenService(newAccessToken);
        var uow = CreateIdentityUnitOfWork();

        const string userId = "user-123";
        const string userEmail = "john@example.com";
        const string incomingToken = "valid-active-token";

        var user = new User
        {
            Id = userId,
            Email = userEmail,
            UserName = "john"
        };

        var tokenHash = hasher.HashToken(incomingToken);
        var storedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        repository.GetByTokenHashAsync(tokenHash, Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(storedToken));

        var userManager = CreateUserManager();
        userManager.FindByIdAsync(userId).Returns(Task.FromResult<User?>(user));
        userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string> { "Admin", "User" }));

        var handler = new RefreshHandler(
            validator,
            userManager,
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest(incomingToken);

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT0.ShouldBeTrue(); // RefreshTokenResponse
        var response = result.AsT0;
        response.AccessToken.ShouldBe(newAccessToken);
        response.RefreshToken.ShouldBe(newRawToken);

        // Verify secure token generator was called
        secureTokenGenerator.Received(1).Generate();

        // Verify access token was generated with correct user and roles
        accessTokenService.Received(1).GenerateAccessToken(
            Is<User>(u => u!.Id == userId),
            Is<List<string>>(r => r!.Count == 2 && r.Contains("Admin") && r.Contains("User")));

        // Verify new token was added to repository
        repository.Received(1).Add(Is<RefreshToken>(t => t!.UserId == userId));

        // Verify old token was updated (rotated)
        repository.Received(1).Update(storedToken);
        storedToken.IsRevoked.ShouldBeTrue();

        // Verify changes were saved
        await uow.Received(1).SaveChangesAsync(Any<CancellationToken>());
    }

    [Fact]
    public async Task Refresh_ValidToken_NewTokenHasCorrectExpiration()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var hasher = CreateRefreshTokenHasher();
        var secureTokenGenerator = CreateSecureTokenGenerator("new-token");
        var accessTokenService = CreateAccessTokenService("new-access-token");
        var uow = CreateIdentityUnitOfWork();

        const string userId = "user-123";
        const int expirationDays = 14;
        const string incomingToken = "valid-token";

        var user = new User { Id = userId, Email = "john@example.com", UserName = "john" };
        var tokenHash = hasher.HashToken(incomingToken);
        var storedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        RefreshToken? capturedNewToken = null;
        repository.GetByTokenHashAsync(tokenHash, Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(storedToken));
        repository.When(x => x.Add(Any<RefreshToken>())).Do(x => capturedNewToken = x.Arg<RefreshToken>());

        var userManager = CreateUserManager();
        userManager.FindByIdAsync(userId).Returns(Task.FromResult<User?>(user));
        userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(new List<string>()));

        var beforeRefresh = DateTime.UtcNow;
        var handler = new RefreshHandler(
            validator,
            userManager,
            CreateRefreshTokenOptions(expirationDays),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest(incomingToken);

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT0.ShouldBeTrue();

        capturedNewToken.ShouldNotBeNull();
        capturedNewToken!.UserId.ShouldBe(userId);
        capturedNewToken.ExpiresAt.ShouldBeGreaterThanOrEqualTo(beforeRefresh.AddDays(expirationDays));
        capturedNewToken.ExpiresAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow.AddDays(expirationDays));
    }

    [Fact]
    public async Task Refresh_ExpiredToken_ReturnsInvalidRefreshTokenError()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var hasher = CreateRefreshTokenHasher();
        var secureTokenGenerator = CreateSecureTokenGenerator();
        var accessTokenService = CreateAccessTokenService();
        var uow = CreateIdentityUnitOfWork();

        const string userId = "user-123";
        const string tokenHash = "hash-of-expired-token";

        var expiredToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddSeconds(-10) // Expired 10 seconds ago
        };

        repository.GetByTokenHashAsync(tokenHash, Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(expiredToken));

        var user = new User { Id = userId, Email = "john@example.com", UserName = "john" };
        var userManager = CreateUserManager();
        userManager.FindByIdAsync(userId).Returns(Task.FromResult<User?>(user));

        var handler = new RefreshHandler(
            validator,
            userManager,
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest("expired-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue(); // InvalidRefreshTokenError
    }

    [Fact]
    public async Task Refresh_RevokedToken_ReturnsInvalidRefreshTokenError()
    {
        // Arrange
        var validator = new RefreshRequestValidator();
        var repository = CreateRefreshTokenRepository();
        var hasher = CreateRefreshTokenHasher();
        var secureTokenGenerator = CreateSecureTokenGenerator();
        var accessTokenService = CreateAccessTokenService();
        var uow = CreateIdentityUnitOfWork();

        const string userId = "user-123";
        const string tokenHash = "hash-of-revoked-token";

        var revokedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        // Revoke the token
        revokedToken.Revoke();

        repository.GetByTokenHashAsync(tokenHash, Any<CancellationToken>())
            .Returns(Task.FromResult<RefreshToken?>(revokedToken));

        var user = new User { Id = userId, Email = "john@example.com", UserName = "john" };
        var userManager = CreateUserManager();
        userManager.FindByIdAsync(userId).Returns(Task.FromResult<User?>(user));

        var handler = new RefreshHandler(
            validator,
            userManager,
            CreateRefreshTokenOptions(),
            secureTokenGenerator,
            accessTokenService,
            repository,
            uow,
            hasher);

        var request = new RefreshRequest("revoked-token");

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.IsT2.ShouldBeTrue(); // InvalidRefreshTokenError
    }
}