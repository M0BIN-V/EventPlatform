using BuildingBlocks.Application;
using FluentValidation;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Errors;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.Login;

public class LoginHandler(
    IValidator<LoginRequest> validator,
    UserManager<User> userManager,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IOptions<JwtOptions> jwtOptions) :
    Handler<LoginRequest, LoginResponse>
{
    public override async Task<LoginResponse> HandleAsync(LoginRequest request, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.Errors;

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return new UserNotFoundError(request.Email);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return new InvalidPasswordError();

        var accessToken = await tokenService.GenerateAccessTokenAsync(user);
        
        // Generate and persist refresh token
        var (refreshToken, _) = await refreshTokenService.GenerateAsync(
            user.Id,
            jwtOptions.Value.RefreshTokenExpirationDays,
            ipAddress: null,
            userAgent: null,
            ct);

        return new LoginTokenResponse(accessToken, refreshToken);
    }
}