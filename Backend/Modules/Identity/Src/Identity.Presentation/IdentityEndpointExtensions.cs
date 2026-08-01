using BuildingBlocks.Presentation.Extensions;
using Identity.Application.Common.Errors;
using Identity.Application.Features.ConfirmEmail;
using Identity.Application.Features.Login;
using Identity.Application.Features.Logout;
using Identity.Application.Features.Refresh;
using Identity.Application.Features.Register;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Presentation;

public static class IdentityEndpointExtensions
{
    private static async Task<Results<
            Ok<string>,
            ValidationProblem,
            NotFound<UserNotFoundError>,
            BadRequest<EmailOrConfirmationTokenIsNotValidError>>>
        ConfirmEmail(
            [FromServices] ConfirmEmailHandler handler,
            [FromQuery] string email,
            [FromQuery] string token)
    {
        var request = new ConfirmEmailRequest(email, token);

        var result = await handler.HandleAsync(request);

        return result
            .Match<Results<
                Ok<string>,
                ValidationProblem,
                NotFound<UserNotFoundError>,
                BadRequest<EmailOrConfirmationTokenIsNotValidError>>>(
                confirmed => Ok(confirmed),
                invalidTokenOrEmailError => BadRequest(invalidTokenOrEmailError),
                validationFailure => validationFailure.ToValidationProblem());
    }

    private static async Task<Results<Created, Conflict<UserAlreadyExistsError>, ValidationProblem>> Register(
        [FromServices] RegisterHandler handler,
        [FromBody] RegisterRequest request)
    {
        var result = await handler.HandleAsync(request);

        return result.Match<Results<Created, Conflict<UserAlreadyExistsError>, ValidationProblem>>(
            userId => Created(),
            validationErrors => validationErrors.ToValidationProblem(),
            userAlreadyExistsError => Conflict(userAlreadyExistsError)
        );
    }

    private static async Task<Results<
        Ok<LoginTokenResponse>,
        BadRequest<InvalidPasswordError>,
        NotFound<UserNotFoundError>,
        IResult,
        ValidationProblem>> Login(
        [FromServices] LoginHandler handler,
        [FromBody] LoginRequest request)
    {
        var result = await handler.HandleAsync(request);

        return result
            .Match<Results<Ok<LoginTokenResponse>, BadRequest<InvalidPasswordError>, NotFound<UserNotFoundError>,
                IResult, ValidationProblem>>(
                tokens => Ok(tokens),
                validationErrors => validationErrors.ToValidationProblem(),
                userNotFoundError => NotFound(userNotFoundError),
                emailNotConfirmedError => Json(emailNotConfirmedError, statusCode: StatusCodes.Status403Forbidden),
                invalidPasswordError => BadRequest(invalidPasswordError)
            );
    }

    private static async Task<Results<Ok<RefreshTokenResponse>, UnauthorizedHttpResult, ValidationProblem>> Refresh(
        [FromServices] RefreshHandler handler,
        [FromBody] RefreshRequest request)
    {
        var result = await handler.HandleAsync(request);

        return result.Match<Results<Ok<RefreshTokenResponse>, UnauthorizedHttpResult, ValidationProblem>>(
            tokens => Ok(tokens),
            validationErrors => validationErrors.ToValidationProblem(),
            invalidTokenError => Unauthorized()
        );
    }

    private static async Task<Results<Ok<LogoutSuccessResponse>, UnauthorizedHttpResult, ValidationProblem>> Logout(
        [FromServices] LogoutHandler handler,
        [FromBody] LogoutRequest request)
    {
        var result = await handler.HandleAsync(request);

        return result.Match<Results<Ok<LogoutSuccessResponse>, UnauthorizedHttpResult, ValidationProblem>>(
            success => Ok(success),
            invalidRefreshTokenError => Unauthorized(),
            validationErrors => validationErrors.ToValidationProblem()
        );
    }

    public static IEndpointRouteBuilder MapIdentityModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var identityGroup = app.MapGroup("/identity").WithTags("Identity");

        identityGroup.MapPost("/register", Register)
            .WithName("RegisterUser")
            .WithSummary("Registers a new user.")
            .WithDescription("Creates a new user account with the provided details.");

        identityGroup.MapPost("/login", Login)
            .WithName("LoginUser")
            .WithSummary("Logs in a user.")
            .WithDescription("Authenticates a user and returns JWT and refresh tokens.");

        identityGroup.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .WithSummary("Refreshes authentication tokens.")
            .WithDescription("Uses a refresh token to obtain a new access token and refresh token.");

        identityGroup.MapPost("/logout", Logout)
            .WithName("LogoutUser")
            .WithSummary("Logs out a user.")
            .WithDescription("Revokes the current refresh token.");

        identityGroup.MapGet("confirm-email", ConfirmEmail)
            .WithName("ConfirmEmail")
            .WithSummary("Confirms email address.")
            .WithDescription("Confirms email address.");

        return app;
    }
}