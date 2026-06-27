using BuildingBlocks.Application.Events;
using Microsoft.Extensions.Logging;

namespace Notification.Application.Features;

public class ConfirmEmailRequestedEventHandler(ILogger<ConfirmEmailRequestedEventHandler> logger)
{
    public async Task Handle(ConfirmEmailRequestedEvent @event)
    {
        logger.LogInformation($"Received ConfirmEmailRequested event '{@event.Email}'");
        await Task.Delay(4000);
        logger.LogInformation($"Completed ConfirmEmailRequested event '{@event.Email}'");
    }
}