using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BuildingBlocks.Presentation.Extensions;

public static class ValidationFailureExtensions
{
    public static ValidationProblem ToValidationProblems(this List<ValidationFailure> problems)
    {
        var problemsDictionary = problems.GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        return ValidationProblem(problemsDictionary);
    }
}