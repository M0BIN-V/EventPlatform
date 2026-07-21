using Identity.Domain.Entities;
using OneOf;
using BuildingBlocks.Application;
using Identity.Application.Errors;

namespace Identity.Application.Common.Contracts.Services;

public interface IRefreshTokenService
{
    Task<(string RawToken, RefreshToken Entity)> GenerateAsync(
        string userId,
        int expirationDays,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken ct = default);
    
    Task<OneOf<(User User, RefreshToken Token), InvalidRefreshTokenError>> ValidateAsync(
        string refreshToken,
        CancellationToken ct = default);


    Task<(string RawToken, RefreshToken NewToken)> RotateAsync(
        RefreshToken oldToken,
        int expirationDays,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken ct = default);

    Task RevokeAsync(RefreshToken token, CancellationToken ct = default);


    Task RevokeAllAsync(string userId, CancellationToken ct = default);


    Task CleanupExpiredAsync(CancellationToken ct = default);
}

