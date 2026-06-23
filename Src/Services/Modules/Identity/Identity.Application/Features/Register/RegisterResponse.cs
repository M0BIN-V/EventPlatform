using FluentValidation.Results;
using Identity.Application.Errors;
using OneOf;

namespace Identity.Application.Features.Register;

[GenerateOneOf]
public partial class RegisterResponse : OneOfBase<
    string,
    List<ValidationFailure>,
    UserAlreadyExistsError
>;