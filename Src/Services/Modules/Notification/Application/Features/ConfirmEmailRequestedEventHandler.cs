using Microsoft.Extensions.Logging;

namespace Application.Features;

public class ConfirmEmailRequestedEventHandler(ILogger<ConfirmEmailRequestedEventHandler> logger)
{
    public Task Handler()
    {
        logger.LogInformation("Received ConfirmEmailRequested event");
        return Task.CompletedTask;
    }
}