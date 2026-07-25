namespace Identity.Application.Common.Errors;

public record InvalidRefreshTokenError(string Message = "Invalid refresh token")
    : Error(nameof(InvalidRefreshTokenError), Message);

public record TokenAlreadyRotatedError() : Error(nameof(TokenAlreadyRotatedError), "Token is already rotated");