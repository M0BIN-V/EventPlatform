using System.Security.Claims;

namespace Identity.Application.Common.Contracts.Services;

public interface IAccessTokenService
{
    string GenerateAccessToken(User user,List< string> roles);
}
