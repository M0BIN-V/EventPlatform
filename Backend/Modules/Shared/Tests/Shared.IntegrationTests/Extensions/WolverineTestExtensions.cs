using Shared.IntegrationTests.Common;
using Wolverine.Tracking;

namespace Shared.IntegrationTests.Extensions;

public static class WolverineTestExtensions
{
    public static async Task<(TResult Result, ITrackedSession Tracked)> TrackAsync<TResult>(
        this IntegrationTestFixture fixture,
        Func<Task<TResult>> action)
    {
        ITrackedSession tracked = null!;
        TResult result = default!;

        tracked = await fixture.TrackWolverineAsync(async () => { result = await action(); });

        return (result, tracked);
    }
}