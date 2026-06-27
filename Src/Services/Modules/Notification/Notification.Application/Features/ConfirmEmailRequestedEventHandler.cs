using BuildingBlocks.Application.Events;
using FluentEmail.Core;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Application.Options;

namespace Notification.Application.Features;

public class ConfirmEmailRequestedEventHandler(
    IFluentEmailFactory emailFactory,
    ILogger<ConfirmEmailRequestedEventHandler> logger)
{
    public async Task Handle(ConfirmEmailRequestedEvent @event)
    {
        logger.LogInformation($"Received ConfirmEmailRequested event");
        
        var email = emailFactory.Create()
            .To(@event.Email)
            .Subject("Confirm your email")
            .Body($"Please confirm your email by clicking the following link: {@event.ConfirmationUrl}");

        var result = await email.SendAsync();
        
        if(!result.Successful)
        {
            logger.LogError("Failed to send confirmation email");
        }

        logger.LogInformation($"Completed ConfirmEmailRequested event");
    }
}