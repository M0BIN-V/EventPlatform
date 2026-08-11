using BuildingBlocks.Application;

namespace Organization.Application.Common.Errors;

public record OrganizationNameRequiredError()
    : Error(nameof(OrganizationNameRequiredError), "Organization name is required.");

public record OrganizationSlugRequiredError()
    : Error(nameof(OrganizationSlugRequiredError), "Organization slug is required.");

public record InvalidOrganizationSlugError(string Slug)
    : Error(nameof(InvalidOrganizationSlugError), $"Organization slug '{Slug}' is invalid. Slug must contain only lowercase letters, numbers, and hyphens.");

public record OrganizationSlugAlreadyExistsError(string Slug)
    : Error(nameof(OrganizationSlugAlreadyExistsError), $"Organization with slug '{Slug}' already exists.");
