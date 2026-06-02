using Application.Contracts;
using Application.Contracts.Services;
using BuildingBlocks.Application;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Register;

public record UserAlreadyExistsError(string Email);

public class RegisterUserHandler(
    IIdentityDbContext db,
    IValidator<RegisterUserRequest> validator,
    IPasswordHasher passwordHasher)
    : Handler<RegisterUserRequest, RegisterUserResponse>
{
    public override async Task<RegisterUserResponse> HandleAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid) return validationResult.Errors;

        if (await db.Users.AnyAsync(u => u.Email == request.Email, ct))
            return new UserAlreadyExistsError(request.Email);

        var passwordHash = passwordHasher.Hash(request.Password);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return "User registered successfully.";
    }
}