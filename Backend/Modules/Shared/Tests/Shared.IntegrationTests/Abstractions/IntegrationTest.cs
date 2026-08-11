using Identity.Application.Features.Register;
using Microsoft.AspNetCore.WebUtilities;
using Shared.IntegrationTests.Extensions;
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

    protected async Task RegisterAndConfirmEmailAsync(RegisterRequest request)
    {
        var (_, tracked) = await TestFixture.TrackAsync(() => Client.RegisterUserAsync(request));

        var confirmationUrl = tracked.Sent.SingleMessage<ConfirmEmailRequestedEvent>().ConfirmationUrl;
        var uri = new Uri(confirmationUrl);
        var query = QueryHelpers.ParseQuery(uri.Query);
        var token = query["token"];

        await Client.ConfirmEmailAsync(request.Email, token!);
    }
}