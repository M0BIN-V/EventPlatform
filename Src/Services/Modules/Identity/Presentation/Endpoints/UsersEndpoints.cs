using Application.Users.Register;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints;

public static class UsersEndpoints
{
    static readonly Delegate RegisterUser =
        async (RegisterUserRequest request, [FromServices] RegisterUserHandler handler) =>
        {
            var result = await handler.HandleAsync(request);

            return result.Match<Results<
                ValidationProblem,
                Conflict<string>,
                Created>>(
                createdMessage => Created(),
                errors => errors.ToValidationProblems(),
                alreadyExists => Conflict(alreadyExists.Email)
            );
        };

    public static WebApplication MapUsersEndpoints(this WebApplication app)
    {
        var usersGroup = app.MapGroup("/users").WithTags("Users");

        usersGroup.MapPost("/register", RegisterUser)
            .WithName("Register new user")
            .WithDescription("Register new user")
            .WithSummary("Register new user");

        return app;
    }
}