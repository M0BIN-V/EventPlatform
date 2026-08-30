using BuildingBlocks.Infrastructure;
using Files.Application.Common.Contracts.Persistence;

namespace Files.Infrastructure.Persistence;

public class FilesUnitOfWork(EfFilesDbContext context) : UnitOfWork(context), IFilesUnitOfWork;