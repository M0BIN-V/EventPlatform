using Identity.Application.Common.Contracts.ApplicationServices;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Features.Refresh;
using Identity.Domain.Entities;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Application.UnitTests.Features;

public class RefreshHandlerUnitTests
{
    private readonly IAccessTokenService _accessTokenService = For<IAccessTokenService>();
    private readonly RefreshHandler _handler;
    private readonly IRefreshTokenHasher _hasher = For<IRefreshTokenHasher>();
    private readonly IRefreshTokenManager _refreshTokenManager = For<IRefreshTokenManager>();
    private readonly UserManager<User> _userManager ;
    private readonly RefreshRequestValidator _validator = new();


    public RefreshHandlerUnitTests()
    {
        _userManager = new UserManager<User>()
        
        _handler = new RefreshHandler(
            _refreshTokenManager,
            _validator,
            _userManager,
            _accessTokenService,
            For<IIdentityUnitOfWork>(),
            _hasher);
    }

    [Fact]
    public async Task EmptyRefreshToken_ShouldReturnValidationError()
    {
        //Arrange 
        var request = new RefreshRequest("");

        //Act
        var result = await _handler.HandleAsync(request);

        //Assert
        result.Value.ShouldBeOfType<List<ValidationProblem>>();
    }
}