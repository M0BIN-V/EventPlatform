using BuildingBlocks.Application.Contracts;

namespace Organizations.Application.Common.Contracts.Persistence;

public interface IOrganizationUnitOfWork : IDbContextBase
{
    IOrganizationRepository Organizations { get; }
    IOrganizationMemberRepository Members { get; }
}