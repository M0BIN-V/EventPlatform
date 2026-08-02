using BuildingBlocks.Infrastructure;

namespace WebApi.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task InitializeModulesAsync()
        {
            using var scope = app.Services.CreateScope();

            var moduleInitializers = scope.ServiceProvider
                .GetServices<IModuleInitializer>().ToList();

            foreach (var initializer in moduleInitializers) await initializer.InitializeAsync();
        }
    }
}