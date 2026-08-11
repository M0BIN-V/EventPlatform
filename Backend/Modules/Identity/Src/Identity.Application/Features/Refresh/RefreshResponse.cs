using FluentValidation.Results;

namespace Identity.Application.Features.Refresh;

public record RefreshTokenResponse(string AccessToken, string RefreshToken);

[GenerateOneOf]
public partial class RefreshResponse : OneOfBase<
    RefreshTokenResponse,
    List<ValidationFailure>,
    InvalidRefreshTokenError
>;