using Files.Domain.Entities;
using Files.Infrastructure.Persistence.DbContext;
using Files.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using File = Files.Domain.Entities.File;

namespace Files.Infrastructure.Tests.Persistence;

public class FilesRepositoryTests
{
    [Fact]
    public async Task AddAndRetrieveFile_Works()
    {
        var options = new DbContextOptionsBuilder<EfFilesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new EfFilesDbContext(options);

        var repo = new FilesRepository(context);

        var file = File.CreatePending(
            $"files/{Guid.NewGuid()}",
            "logo.png",
            "image/png",
            FilePurpose.OrganizationLogo);

        await repo.AddAsync(file);
        await context.SaveChangesAsync();

        var fetched = await repo.GetByIdAsync(file.Id);

        fetched.ShouldNotBeNull();
        fetched!.ObjectKey.ShouldBe(file.ObjectKey);
        fetched.FileName.ShouldBe(file.FileName);
        fetched.ContentType.ShouldBe(file.ContentType);
        fetched.Status.ShouldBe(FileStatus.Pending);
    }
}