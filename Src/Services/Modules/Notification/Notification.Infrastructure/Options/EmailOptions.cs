namespace Notification.Infrastructure.Options;

public class EmailOptions
{
    public string DefaultFromEmail { get; set; } = string.Empty;
    public string DefaultFromName { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }

    public int RetryCount { get; set; } = 3;
    public int BackoffBaseSeconds { get; set; } = 2;
}