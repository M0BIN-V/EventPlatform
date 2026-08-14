namespace Organizations.Application.Common.Validators;

public static class ValidatorExtensions
{
    extension<T>(IRuleBuilder<T, string?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, string> IsOrganizationName()
        {
            return ruleBuilder
                .MaximumLength(256);
        }

        public IRuleBuilderOptions<T, string> IsOrganizationSlug()
        {
            return ruleBuilder
                .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");
        }

        public IRuleBuilderOptions<T, string> IsOrganizationDescription()
        {
            return ruleBuilder
                .MaximumLength(500);
        }
    }
}