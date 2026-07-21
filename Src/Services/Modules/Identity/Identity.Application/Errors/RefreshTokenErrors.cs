using BuildingBlocks.Application;

namespace Identity.Application.Errors;

public record InvalidRefreshTokenError(string Message)
    : Error(nameof(InvalidRefreshTokenError), Message);