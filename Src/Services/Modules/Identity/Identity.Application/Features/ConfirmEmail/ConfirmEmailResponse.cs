using FluentValidation.Results;
using Identity.Application.Errors;
using OneOf;

namespace Identity.Application.Features.ConfirmEmail;

[GenerateOneOf]
public partial class ConfirmEmailResponse : OneOfBase<
    string,
    EmailConfirmationFailedError,
    UserNotFoundError,
    List<ValidationFailure>>;