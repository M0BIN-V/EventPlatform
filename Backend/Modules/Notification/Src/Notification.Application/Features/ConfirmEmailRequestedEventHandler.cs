using Identity.Contracts;
using Notification.Application.Contracts.Services;

namespace Notification.Application.Features;

public class ConfirmEmailRequestedEventHandler(
    IEmailService emailService)
{
    public async Task Handle(ConfirmEmailRequestedEvent @event)
    {
        var htmlBody =
            $"""
                 <h1>Hello {@event.FullName}</h1>

                 <p>
                     Please confirm your email.
                 </p>

                 <a href="{@event.ConfirmationUrl}">
                     Confirm Email
                 </a>
             """;


        await emailService.SendAsync(
            @event.Email,
            "Confirm your email",
            htmlBody: htmlBody);
    }
}