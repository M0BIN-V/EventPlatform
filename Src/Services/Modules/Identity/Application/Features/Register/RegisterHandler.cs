using Application.Errors;
using BuildingBlocks.Application;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Register;

public class RegisterHandler(
    IValidator<RegisterRequest> validator,
    UserManager<User> manager) :
    Handler<RegisterRequest, RegisterResponse>
{
    public override async Task<RegisterResponse> HandleAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var existingUser = await manager.FindByEmailAsync(request.Email);
        if (existingUser is not null) return new UserAlreadyExistsError(request.Email);

        var newUser = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        var result = await manager.CreateAsync(newUser, request.Password);
        if (result.Succeeded) return newUser.Id;

        return result.Errors.Select(e => new ValidationFailure(e.Code, e.Description)).ToList();
    }
}