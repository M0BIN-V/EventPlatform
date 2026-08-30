namespace Files.Application.Common.Contracts.Persistence;

public interface IFilesRepository
{
    Task AddAsync(File file, CancellationToken ct = default);
    Task<File?> GetByIdAsync(Guid id, CancellationToken ct = default);
}