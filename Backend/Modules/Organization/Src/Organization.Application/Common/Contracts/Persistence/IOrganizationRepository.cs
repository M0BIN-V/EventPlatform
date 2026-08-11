namespace Organization.Application.Common.Contracts.Persistence;

public interface IOrganizationRepository
{
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
    Task AddAsync(Domain.Entities.Organization organization, CancellationToken ct = default);
    Task<Domain.Entities.Organization?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Domain.Entities.Organization?> GetBySlugAsync(string slug, CancellationToken ct = default);
}