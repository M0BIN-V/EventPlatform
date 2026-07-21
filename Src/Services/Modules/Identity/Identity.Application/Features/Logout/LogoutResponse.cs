using FluentValidation.Results;
using OneOf;

namespace Identity.Application.Features.Logout;

public record LogoutSuccessResponse(string Message);

[GenerateOneOf]
public partial class LogoutResponse : OneOfBase<
    LogoutSuccessResponse,
    List<ValidationFailure>
>;
