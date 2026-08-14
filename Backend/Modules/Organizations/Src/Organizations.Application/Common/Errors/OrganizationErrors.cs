using BuildingBlocks.Application;

namespace Organizations.Application.Common.Errors;

public record OrganizationSlugAlreadyExistsError(string Slug)
    : Error(nameof(OrganizationSlugAlreadyExistsError), $"Organization with slug '{Slug}' already exists.");

public record OrganizationNotFoundError(string Slug)
    : Error(nameof(OrganizationNotFoundError), $"Organization with slug '{Slug}' not found.");

public record OrganizationUnauthorizedError()
    : Error(nameof(OrganizationUnauthorizedError), "You are not authorized to edit this organization.");