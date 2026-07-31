using MailKit.Security;

namespace Notification.Infrastructure.Options;

public class EmailOptions
{
    public string DefaultFromEmail { get; set; } = string.Empty;
    public string DefaultFromName { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 5;
    public SecureSocketOptions Security { get; set; }
}