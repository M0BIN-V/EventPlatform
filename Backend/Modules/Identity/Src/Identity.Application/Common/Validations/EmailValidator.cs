using System.Text.RegularExpressions;

namespace Identity.Application.Common.Validations;

public static partial class EmailValidator
{
    static readonly Regex EmailRegex =
        MyRegex();

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        email = email.Trim();

        return !email.Contains(' ') && EmailRegex.IsMatch(email);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Email is required")
            .Must(EmailValidator.IsValid)
            .WithMessage("Email is not valid");
    }
}