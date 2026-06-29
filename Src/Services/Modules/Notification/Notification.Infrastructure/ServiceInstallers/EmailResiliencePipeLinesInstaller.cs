using DiServiceInstaller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Constants;
using Notification.Infrastructure.Options;
using Notification.Infrastructure.Services;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Notification.Infrastructure.ServiceInstallers;

public class EmailResiliencePipeLinesInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.Services.AddResiliencePipeline(
            ResiliencePipeLineNames.EmailPipeline,
            (pipelineBuilder, context) =>
            {
                var options = context.ServiceProvider
                    .GetRequiredService<IOptions<EmailSettings>>()
                    .Value;

                var logger = context.ServiceProvider
                    .GetRequiredService<ILogger<FluentEmailService>>();

                pipelineBuilder
                    .AddTimeout(TimeSpan.FromSeconds(10))
                    .AddRetry(new RetryStrategyOptions
                    {
                        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                        MaxRetryAttempts = options.RetryCount,
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        Delay = TimeSpan.FromSeconds(options.BackoffBaseSeconds),
                        OnRetry = args =>
                        {
                            logger.LogWarning(
                                args.Outcome.Exception,
                                "Failed to send email. Retry attempt {AttemptCount}. Error: {ErrorMessage}",
                                args.AttemptNumber + 1,
                                args.Outcome.Exception?.Message);

                            return ValueTask.CompletedTask;
                        }
                    })
                    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                    {
                        FailureRatio = 0.5,
                        MinimumThroughput = 10,
                        SamplingDuration = TimeSpan.FromSeconds(30),
                        BreakDuration = TimeSpan.FromMinutes(1),
                        BreakDurationGenerator = null
                    });
            });
    }
}