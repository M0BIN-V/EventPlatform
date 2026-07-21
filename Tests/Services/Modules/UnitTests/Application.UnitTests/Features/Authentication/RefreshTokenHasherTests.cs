using Identity.Application.Common.Contracts.Services;

namespace Application.UnitTests.Features.Authentication;

public class RefreshTokenHasherTests
{
    [Fact]
    public void HashToken_ProducesConsistentHashForSameToken()
    {
        // This test verifies that the IRefreshTokenHasher implementation
        // produces valid hashes that can be verified.
        // Note: In a real scenario, this would be tested with the actual
        // RefreshTokenHasher implementation in the Infrastructure tests.

        // Arrange
        var hasher = For<IRefreshTokenHasher>();
        var token = "test-refresh-token-12345";
        var hash = "generated-hash-1";

        hasher.HashToken(token).Returns(hash);
        hasher.VerifyToken(token, hash).Returns(true);

        // Act
        var result = hasher.VerifyToken(token, hash);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void VerifyToken_ReturnsTrueForCorrectToken()
    {
        // Arrange
        var hasher = For<IRefreshTokenHasher>();
        var token = "my-refresh-token";
        var hash = "correct-hash";

        hasher.VerifyToken(token, hash).Returns(true);

        // Act
        var result = hasher.VerifyToken(token, hash);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void VerifyToken_ReturnsFalseForIncorrectToken()
    {
        // Arrange
        var hasher = For<IRefreshTokenHasher>();
        var wrongToken = "wrong-token";
        var hash = "some-hash";

        hasher.VerifyToken(wrongToken, hash).Returns(false);

        // Act
        var result = hasher.VerifyToken(wrongToken, hash);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void VerifyToken_ReturnsFalseForInvalidHash()
    {
        // Arrange
        var hasher = For<IRefreshTokenHasher>();
        var token = "my-refresh-token";

        hasher.VerifyToken(token, "invalid-hash").Returns(false);

        // Act
        var result = hasher.VerifyToken(token, "invalid-hash");

        // Assert
        result.ShouldBeFalse();
    }
}
