using Files.Application.Common.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Files.Infrastructure.Persistence.Repositories;

public class FilesRepository : IFilesRepository
{
    private readonly EfFilesDbContext _context;

    public FilesRepository(EfFilesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(File file, CancellationToken ct = default)
    {
        await _context.Files.AddAsync(file, ct);
    }

    public async Task<File?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Files.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}