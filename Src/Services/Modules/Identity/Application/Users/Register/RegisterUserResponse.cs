using FluentValidation.Results;
using OneOf;

namespace Application.Users.Register;

[GenerateOneOf]
public partial class RegisterUserResponse : OneOfBase<
    string,
    List<ValidationFailure>,
    UserAlreadyExistsError>;