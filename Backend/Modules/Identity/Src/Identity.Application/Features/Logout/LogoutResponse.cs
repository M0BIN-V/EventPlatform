using FluentValidation.Results;
using Identity.Application.Errors;
using OneOf;

namespace Identity.Application.Features.Logout;

public record LogoutSuccessResponse(string Message);

[GenerateOneOf]
public partial class LogoutResponse : OneOfBase<
    LogoutSuccessResponse,
    InvalidRefreshTokenError,
    List<ValidationFailure>
>;
