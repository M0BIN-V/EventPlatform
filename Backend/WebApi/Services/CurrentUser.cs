using System.Security.Claims;
using BuildingBlocks.Application.Contracts;

namespace WebApi.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException();

    public string Id =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User id claim was not found.");
}