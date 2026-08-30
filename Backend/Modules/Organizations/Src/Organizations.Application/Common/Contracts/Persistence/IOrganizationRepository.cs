using Organizations.Domain.Entities;

namespace Organizations.Application.Common.Contracts.Persistence;

public interface IOrganizationRepository
{
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
    Task AddAsync(Organization organization, CancellationToken ct = default);
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Organization?> GetBySlugAsync(string slug, CancellationToken ct = default);
}