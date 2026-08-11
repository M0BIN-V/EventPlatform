using BuildingBlocks.Infrastructure;
using Identity.Application.Common.Contracts.Persistence;
using Identity.Infrastructure.Persistence.DbContext;

namespace Identity.Infrastructure.Persistence;

public class IdentityUnitOfWork(EfIdentityDbContext context) : UnitOfWork(context), IIdentityUnitOfWork;