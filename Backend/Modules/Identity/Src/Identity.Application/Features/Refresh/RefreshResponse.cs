using FluentValidation.Results;
using Identity.Application.Common.Contracts.Services;

namespace Identity.Application.Features.Refresh;

public record RefreshTokenResponse(string AccessToken, string RefreshToken);

[GenerateOneOf]
public partial class RefreshResponse : OneOfBase<
    RefreshTokenResponse,
    List<ValidationFailure>,
    InvalidRefreshTokenError
>;
