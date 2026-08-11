using BuildingBlocks.Application.Contracts;

namespace Organization.Application.Common.Contracts.Persistence;

public interface IOrganizationUnitOfWork : IDbContextBase
{
    IOrganizationRepository Organizations { get; }
    IOrganizationMemberRepository Members { get; }
}
