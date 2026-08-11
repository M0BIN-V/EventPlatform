using BuildingBlocks.Application;

namespace Organizations.Application.Common.Errors;

public record OrganizationSlugAlreadyExistsError(string Slug)
    : Error(nameof(OrganizationSlugAlreadyExistsError), $"Organization with slug '{Slug}' already exists.");