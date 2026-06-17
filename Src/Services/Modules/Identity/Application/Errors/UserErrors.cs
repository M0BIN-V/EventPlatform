using BuildingBlocks.Application;

namespace Application.Errors;

public record UserAlreadyExistsError(string Email)
    : Error(nameof(UserAlreadyExistsError), $"User with email '{Email}' already exists.");