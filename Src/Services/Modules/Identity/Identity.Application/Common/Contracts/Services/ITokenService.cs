using Identity.Domain.Entities;
using System.Security.Claims;

namespace Identity.Application.Common.Contracts.Services;

/// <summary>
/// Service for generating JWT access tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the given user.
    /// </summary>
    Task< string> GenerateAccessTokenAsync(User user);
}
