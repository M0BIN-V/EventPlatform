using System.Net.Sockets;
using DiServiceInstaller;
using JasperFx.Core;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Constants;
using Notification.Infrastructure.Options;
using Polly;
using Polly.Retry;
using Polly.Timeout;

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
                    .GetRequiredService<IOptions<EmailOptions>>()
                    .Value;

                pipelineBuilder
                    .AddRetry(new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        Delay = 2.Seconds(),
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        ShouldHandle = new PredicateBuilder()
                            .Handle<TimeoutRejectedException>()
                            .Handle<SocketException>()
                            .Handle<SmtpProtocolException>()
                    })
                    .AddTimeout(options.TimeoutSeconds.Seconds());
            });
    }
}