using Application.Errors;
using FluentValidation.Results;
using OneOf;

namespace Application.Features.Register;

[GenerateOneOf]
public partial class RegisterResponse : OneOfBase<
    string,
    List<ValidationFailure>,
    UserAlreadyExistsError
>;