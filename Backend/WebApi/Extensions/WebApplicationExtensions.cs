using BuildingBlocks.Infrastructure;

namespace WebApi.Extensions;

public static partial class WebApplicationExtensions
{
    [LoggerMessage(LogLevel.Information, "Initializing {Count} module initializers...")]
    static partial void LogInitializingCountModuleInitializers(ILogger<WebApplication> logger, int count);

    extension(WebApplication app)
    {
        public async Task InitializeModulesAsync()
        {
            using var scope = app.Services.CreateScope();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();

            var moduleInitializers = scope.ServiceProvider
                .GetServices<IModuleInitializer>().ToList();

            LogInitializingCountModuleInitializers(logger, moduleInitializers.Count);
            
            foreach (var initializer in moduleInitializers)
            {
                await initializer.InitializeAsync();
            }
            
            logger.LogInformation("Module initialization completed.");
        }
    }
}