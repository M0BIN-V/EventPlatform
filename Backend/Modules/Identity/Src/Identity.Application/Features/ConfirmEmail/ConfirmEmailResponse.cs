using FluentValidation.Results;

namespace Identity.Application.Features.ConfirmEmail;

[GenerateOneOf]
public partial class ConfirmEmailResponse : OneOfBase<
    string,
    EmailOrConfirmationTokenIsNotValidError,
    List<ValidationFailure>>;