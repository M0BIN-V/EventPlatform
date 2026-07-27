using Identity.Domain.Constants;

namespace Application.UnitTests.Domain.Entities;

public class RefreshTokenTests
{
    [Fact]
    public void IsExpired_WhenRefreshTokenIsExpired_ShouldReturnTrue()
    {
        //Arrange 
        var now = DateTime.UtcNow;
        var refreshToken = new RefreshToken
        {
            CreatedAt = now.AddDays(-5),
            ExpiresAt = now.AddDays(-1),
            UserId = "this is user id",
            TokenHash = "this is token hash",
        };
        
        //act 
        var result = refreshToken.IsExpired(now);
        
        //Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void IsExpired_WhenRefreshTokenIsNotExpired_ShouldReturnFalse()
    {
        //Arrange 
        var now = DateTime.UtcNow;
        var refreshToken = new RefreshToken
        {
            CreatedAt = now.AddDays(-5),
            ExpiresAt = now.AddDays(1),
            UserId = "this is user id",
            TokenHash = "this is token hash",
        };
        
        //act 
        var result = refreshToken.IsExpired(now);
        
        //Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void IsRevoked_WhenRefreshTokenIsNotRevoked_ShouldReturnFalse()
    {
        //Arrange 
        var now = DateTime.UtcNow;
        var refreshToken = new RefreshToken
        {
            CreatedAt = now.AddDays(-5),
            ExpiresAt = now.AddDays(1),
            UserId = "this is user id",
            TokenHash = "this is token hash",
        };
        
        //act 
        var result = refreshToken.IsRevoked();
        
        //Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void IsRevoked_WhenRefreshTokenIsRevoked_ShouldReturnTrue()
    {
        //Arrange 
        var now = DateTime.UtcNow;
        var refreshToken = new RefreshToken
        {
            CreatedAt = now.AddDays(-5),
            ExpiresAt = now.AddDays(1),
            UserId = "this is user id",
            TokenHash = "this is token hash",
        };
        
        refreshToken.Revoke(now, RevocationReason.Logout);
        
        //act 
        var result = refreshToken.IsRevoked();
        
        //Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void IsReplaced_WhenRefreshTokenIsRotated_ShouldReturnTrue()
    {
        //Arrange 
        var now = DateTime.UtcNow;
        var refreshToken = new RefreshToken
        {
            CreatedAt = now.AddDays(-5),
            ExpiresAt = now.AddDays(1),
            UserId = "this is user id",
            TokenHash = "this is token hash",
        };

        var newRefreshToken = new RefreshToken
        {
            CreatedAt = now,
            ExpiresAt = now.AddDays(7),
            UserId = "this is user id",
            TokenHash = "this is new token hash",
        };
        
        refreshToken.Rotate(refreshToken,now);
        
        //act 
        var result = refreshToken.IsReplaced();
        
        //Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void IsReplaced_WhenRefreshTokenIsNotRotated_ShouldReturnFalse()
    {
        //Arrange 
        var now = DateTime.UtcNow;
        var refreshToken = new RefreshToken
        {
            CreatedAt = now.AddDays(-5),
            ExpiresAt = now.AddDays(1),
            UserId = "this is user id",
            TokenHash = "this is token hash",
        };

        var newRefreshToken = new RefreshToken
        {
            CreatedAt = now,
            ExpiresAt = now.AddDays(7),
            UserId = "this is user id",
            TokenHash = "this is new token hash",
        };
        
        //act 
        var result = refreshToken.IsReplaced();
        
        //Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsActive_WhenRefreshTokenIsRotated_ShouldReturnFalse()
    {
        //Arrange 
        var now = DateTime.UtcNow;
        var refreshToken = new RefreshToken
        {
            CreatedAt = now.AddDays(-5),
            ExpiresAt = now.AddDays(1),
            UserId = "this is user id",
            TokenHash = "this is token hash",
        };

        var newRefreshToken = new RefreshToken
        {
            CreatedAt = now,
            ExpiresAt = now.AddDays(7),
            UserId = "this is user id",
            TokenHash = "this is new token hash",
        };
        
        refreshToken.Rotate(newRefreshToken, now);
        
        //act 
        var result = refreshToken.IsActive(now);
        
        //Assert
        result.ShouldBeFalse();
    }
    
}