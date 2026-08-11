using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuildingBlocks.Application.Contracts;

namespace WebApi.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Id
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(JwtRegisteredClaimNames.Sub);

            return userId!;
        }
    }
}