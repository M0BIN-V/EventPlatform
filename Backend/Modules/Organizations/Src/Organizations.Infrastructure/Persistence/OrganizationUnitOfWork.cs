using BuildingBlocks.Infrastructure;

namespace Organizations.Infrastructure.Persistence;

public class OrganizationUnitOfWork(EfOrganizationDbContext context) : UnitOfWork(context), IOrganizationUnitOfWork;