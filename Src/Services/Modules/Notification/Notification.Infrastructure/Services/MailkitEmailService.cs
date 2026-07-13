using JasperFx.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Application.Contracts.Services;
using Notification.Infrastructure.Constants;
using Notification.Infrastructure.Options;
using Polly;
using Polly.Registry;

namespace Notification.Infrastructure.Services;

public class MailkitEmailService(
    IOptions<EmailOptions> options,
    ResiliencePipelineProvider<string> pipelineProvider)
    : IEmailService
{
    private readonly ResiliencePipeline _pipeline = pipelineProvider.GetPipeline(ResiliencePipeLineNames.EmailPipeline);

    public async Task SendAsync(string emailAddress, string subject, string message,
        CancellationToken cancellationToken = default)
    {
        var emailOptions = options.Value;

        await _pipeline.ExecuteAsync(async ct =>
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(emailOptions.DefaultFromName, emailOptions.DefaultFromEmail));
            email.To.Add(MailboxAddress.Parse(emailAddress));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { TextBody = message };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                //TODO : improve security configurations
                
                await smtp.ConnectAsync(emailOptions.SmtpServer, emailOptions.Port, SecureSocketOptions.Auto, ct);

                if (emailOptions.Username.IsNotEmpty() && emailOptions.Password.IsNotEmpty())
                    await smtp.AuthenticateAsync(emailOptions.Username, emailOptions.Password, ct);

                await smtp.SendAsync(email, ct);
            }
            finally
            {
                if (smtp.IsConnected)
                    await smtp.DisconnectAsync(true, CancellationToken.None);
            }
        }, cancellationToken);
    }
}