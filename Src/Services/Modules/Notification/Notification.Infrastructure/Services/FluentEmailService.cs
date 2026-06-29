using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Notification.Application.Contracts.Services;
using Notification.Infrastructure.Common.Exceptions;
using Notification.Infrastructure.Constants;
using Polly;
using Polly.Registry;

namespace Notification.Infrastructure.Services;

public class FluentEmailService(
    IFluentEmailFactory fluentEmailFactory,
    ILogger<FluentEmailService> logger,
    ResiliencePipelineProvider<string> pipelineProvider)
    : IEmailService
{
    private readonly ResiliencePipeline _pipeline = pipelineProvider.GetPipeline(ResiliencePipeLineNames.EmailPipeline);

    public async Task SendAsync(string emailAddress, string subject, string message,
        CancellationToken cancellationToken = default)
    {
        await _pipeline.ExecuteAsync(async token =>
        {
            var email = fluentEmailFactory.Create()
                .To(emailAddress)
                .Subject(subject)
                .Body(message);

            var result = await email.SendAsync(token);

            if (!result.Successful)
            {
                var errorDetails = string.Join(", ", result.ErrorMessages);

                logger.LogError("FluentEmail delivery failed for {EmailAddress}. Errors: {Errors}", emailAddress,
                    errorDetails);
                throw new EmailDeliveryException($"FluentEmail failed. Errors: {errorDetails}");
            }
        }, cancellationToken);
    }
}