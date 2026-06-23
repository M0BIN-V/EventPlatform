using BuildingBlocks.Application;

namespace Identity.Application.Errors;

public record UserAlreadyExistsError(string Email)
    : Error(nameof(UserAlreadyExistsError), $"User with email '{Email}' already exists.");