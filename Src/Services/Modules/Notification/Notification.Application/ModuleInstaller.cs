using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Application.Options;

namespace Notification.Application;

public static class ModuleInstaller
{
    public static IHostApplicationBuilder AddNotificationModule(this IHostApplicationBuilder builder)
    {
        var emailSettings = builder.Configuration
            .GetSection("EmailSettings")
            .Get<EmailSettings>() ?? throw new InvalidOperationException(
            "EmailSettings section is missing in the configuration.");

        var smtpClient = new SmtpClient(emailSettings.SmtpServer)
        {
            Port = emailSettings.Port,
            Credentials = string.IsNullOrEmpty(emailSettings.Username)
                ? null
                : new NetworkCredential(emailSettings.Username, emailSettings.Password),
            EnableSsl = emailSettings.EnableSsl
        };

        builder.Services
            .AddFluentEmail(emailSettings.DefaultFromEmail, emailSettings.DefaultFromName)
            .AddSmtpSender(smtpClient);

        return builder;
    }
}