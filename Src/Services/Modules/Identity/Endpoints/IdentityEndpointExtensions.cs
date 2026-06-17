using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Endpoints;

public static class IdentityEndpointExtensions
{
    static async Task<Results<Created, Conflict<string>, BadRequest<IEnumerable<IdentityError>>>> Register(
        [FromServices] UserManager<User> manager,
        RegisterUserDto request)
    {
        var existingUser = await manager.FindByEmailAsync(request.Email);

        if (existingUser is not null) return Conflict("A user with the provided email already exists.");

        var newUser = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        var result = await manager.CreateAsync(newUser, request.Password);

        if (result.Succeeded) return Created();

        return BadRequest(result.Errors);
    }

    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        var identityGroup = app.MapGroup("/identity").WithTags("Identity");

        identityGroup.MapPost("/register", Register)
            .WithName("RegisterUser")
            .WithSummary("Registers a new user.")
            .WithDescription("Creates a new user account with the provided details.");

        return app;
    }
}