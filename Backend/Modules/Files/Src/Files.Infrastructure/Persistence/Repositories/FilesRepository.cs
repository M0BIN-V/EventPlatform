using Files.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using File = Files.Domain.Entities.File;

namespace Files.Infrastructure.Persistence.Repositories;

public class FilesRepository(EfFilesDbContext context)
{
    public async Task AddAsync(File file, CancellationToken ct = default)
    {
        await context.Files.AddAsync(file, ct);
    }

    public async Task<File?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Files.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}