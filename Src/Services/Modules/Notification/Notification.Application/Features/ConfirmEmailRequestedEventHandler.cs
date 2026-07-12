using BuildingBlocks.Application.Events;
using Notification.Application.Contracts.Services;

namespace Notification.Application.Features;

public class ConfirmEmailRequestedEventHandler(
    IEmailService emailService)
{
    public async Task Handle(ConfirmEmailRequestedEvent @event)
    {
        await emailService.SendAsync(
            @event.Email,
            "Confirm your email",
            $"Please confirm your email by clicking the following link: {@event.ConfirmationUrl}");
    }
}