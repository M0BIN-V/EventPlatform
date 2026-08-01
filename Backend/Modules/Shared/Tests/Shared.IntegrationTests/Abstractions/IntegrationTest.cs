using Shared.IntegrationTests.Fixtures;

namespace Shared.IntegrationTests.Abstractions;

public abstract class IntegrationTest(IntegrationTestFixture testFixture) : IAsyncLifetime
{
    protected readonly HttpClient Client = testFixture.CreateClient();
    protected readonly IntegrationTestFixture TestFixture = testFixture;

    public async Task InitializeAsync()
    {
        await TestFixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}