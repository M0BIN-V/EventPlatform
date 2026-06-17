using System;

namespace Endpoints.Application.Dtos;

public record RegisterUserDto(string? FirstName, string? LastName, string Email, string Password);
