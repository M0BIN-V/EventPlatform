using FluentValidation.Results;
using Identity.Application.Common.Contracts.Services;
using Identity.Application.Errors;
using OneOf;

namespace Identity.Application.Features.Refresh;

public record RefreshTokenResponse(string AccessToken, string RefreshToken);

[GenerateOneOf]
public partial class RefreshResponse : OneOfBase<
    RefreshTokenResponse,
    List<ValidationFailure>,
    InvalidRefreshTokenError
>;
