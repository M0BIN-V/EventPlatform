using FluentValidation.Results;

namespace Identity.Application.Features.Login;

public record LoginTokenResponse(string AccessToken, string RefreshToken);

[GenerateOneOf]
public partial class LoginResponse : OneOfBase<
    LoginTokenResponse,
    List<ValidationFailure>,
    UserNotFoundError,
    InvalidPasswordError
>;
