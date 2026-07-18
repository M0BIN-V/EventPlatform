namespace Notification.Application.Contracts.Services;

public interface IEmailService
{
    public Task SendAsync(
        string emailAddress,
        string subject,
        string? textBody = null,
        string? htmlBody = null,
        CancellationToken ct = default);
}