using FluentValidation.Results;

namespace Identity.Application.Features.ConfirmEmail;

[GenerateOneOf]
public partial class ConfirmEmailResponse : OneOfBase<
    string,
    EmailConfirmationFailedError,
    UserNotFoundError,
    List<ValidationFailure>>;