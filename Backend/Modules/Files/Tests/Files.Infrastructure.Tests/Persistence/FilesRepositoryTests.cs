using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using Files.Infrastructure.Persistence.DbContext;
using Files.Infrastructure.Persistence.Repositories;
using Files.Domain.Entities;

namespace Files.Infrastructure.Tests.Persistence;

public class FilesRepositoryTests
{
    [Fact]
    public async Task AddAndRetrieveFile_Works()
    {
        var options = new DbContextOptionsBuilder<EfFilesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new EfFilesDbContext(options);

        var repo = new FilesRepository(context);

        var file = Files.Domain.Entities.File.CreatePending(
            objectKey: $"files/{Guid.NewGuid()}",
            fileName: "logo.png",
            contentType: "image/png",
            purpose: Files.Domain.Entities.FilePurpose.OrganizationLogo);

        await repo.AddAsync(file);
        await context.SaveChangesAsync();

        var fetched = await repo.GetByIdAsync(file.Id);

        fetched.ShouldNotBeNull();
        fetched!.ObjectKey.ShouldBe(file.ObjectKey);
        fetched.FileName.ShouldBe(file.FileName);
        fetched.ContentType.ShouldBe(file.ContentType);
        fetched.Status.ShouldBe(Files.Domain.Entities.FileStatus.Pending);
    }
}
