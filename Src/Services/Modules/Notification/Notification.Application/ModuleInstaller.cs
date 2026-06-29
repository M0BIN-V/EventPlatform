using FluentEmail.MailKitSmtp;
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

        var smtpClientOptions = new SmtpClientOptions
        {
            Server = emailSettings.SmtpServer,
            Port = emailSettings.Port,
            Password = emailSettings.Password,
            User = emailSettings.Username
        };

        builder.Services
            .AddFluentEmail(emailSettings.DefaultFromEmail, emailSettings.DefaultFromName)
            .AddMailKitSender(smtpClientOptions);

        return builder;
    }
}