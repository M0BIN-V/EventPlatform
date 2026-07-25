namespace Identity.Application.Common.Errors;

public record UserAlreadyExistsError(string Email)
    : Error(nameof(UserAlreadyExistsError), $"User with email '{Email}' already exists.");

public record UserNotFoundError(string Email)
    : Error(nameof(UserNotFoundError), $"User with email '{Email}' does not exist.");

public record InvalidPasswordError()
    : Error(nameof(InvalidPasswordError), "Email or password is incorrect.");