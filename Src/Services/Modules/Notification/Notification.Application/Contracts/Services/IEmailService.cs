namespace Notification.Application.Contracts.Services;

public interface IEmailService
{
    public Task SendAsync(
        string emailAddress,
        string subject,
        string message,
        CancellationToken cancellationToken = default);
}