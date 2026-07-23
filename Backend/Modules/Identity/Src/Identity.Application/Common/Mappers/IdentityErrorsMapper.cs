using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Common.Mappers;

public static class IdentityErrorsMapper
{
    public static List<ValidationFailure> ToValidationFailure(this IEnumerable<IdentityError> errors)
    {
        return errors.Select(e =>
            new ValidationFailure(e.Code, e.Description)
            {
                ErrorCode = e.Code
            }).ToList();
    }
}