using BuildingBlocks.Application;

namespace Identity.Application.Errors;

public record InvalidRefreshTokenError(string Message = "Invalid refresh token")
    : Error(nameof(InvalidRefreshTokenError), Message);