using FluentValidation.Results;

namespace Identity.Application.Features.Register;

[GenerateOneOf]
public partial class RegisterResponse : OneOfBase<
    string,
    List<ValidationFailure>,
    UserAlreadyExistsError
>;