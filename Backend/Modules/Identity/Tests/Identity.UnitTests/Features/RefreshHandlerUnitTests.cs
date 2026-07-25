using FluentValidation.Results;
using Identity.Application.Common.Contracts.ApplicationServices;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Errors;
using Identity.Application.Features.Logout;
using Identity.Domain.Constants;

namespace Application.UnitTests.Features;

public class RefreshHandlerUnitTests
{
    private readonly LogoutHandler _handler;
    private readonly IRefreshTokenManager _refreshTokenManager;
    private readonly LogoutRequestValidator _validator = new();

    public RefreshHandlerUnitTests()
    {
        _refreshTokenManager = For<IRefreshTokenManager>();
        var uow = For<IIdentityUnitOfWork>();

        _handler = new LogoutHandler(
            _refreshTokenManager,
            _validator,
            uow);
    }


    [Fact]
    public async Task HandleAsync_ShouldRevokeTokenAndReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var request = new LogoutRequest("refresh-token");

        _refreshTokenManager
            .RevokeAsync(
                request.RefreshToken,
                RevocationReason.Logout,
                Any<CancellationToken>())
            .Returns((InvalidRefreshTokenError?)null);


        // Act
        var result = await _handler.HandleAsync(request);


        // Assert
        result.Value.ShouldBeOfType<LogoutSuccessResponse>();
    }


    [Fact]
    public async Task HandleAsync_Should_Return_Validation_Error_When_Request_Is_Invalid()
    {
        // Arrange
        var request = new LogoutRequest("");

        // Act
        var result = await _handler.HandleAsync(request);


        // Assert
        result.Value.ShouldBeOfType<List<ValidationFailure>>();
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Error_When_RefreshToken_Is_Invalid()
    {
        // Arrange
        var request = new LogoutRequest("invalid-token");
        
        _refreshTokenManager
            .RevokeAsync(
                request.RefreshToken,
                RevocationReason.Logout,
                Any<CancellationToken>())
            .Returns(new InvalidRefreshTokenError());


        // Act
        var result = await _handler.HandleAsync(request);


        // Assert
        result.Value.ShouldBeOfType<InvalidRefreshTokenError>();
    }
}