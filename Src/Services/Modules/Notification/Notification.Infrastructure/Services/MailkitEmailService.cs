using JasperFx.Core;
using MailKit.Net.Smtp;
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

    public async Task SendAsync(
        string emailAddress,
        string subject,
        string? textBody = null,
        string? htmlBody = null,
        CancellationToken ct = default)
    {
        var emailOptions = options.Value;

        await _pipeline.ExecuteAsync(async ect =>
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(emailOptions.DefaultFromName, emailOptions.DefaultFromEmail));
            email.To.Add(MailboxAddress.Parse(emailAddress));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(emailOptions.SmtpServer, emailOptions.Port, emailOptions.Security, ect);

                if (emailOptions.Username.IsNotEmpty() && emailOptions.Password.IsNotEmpty())
                    await smtp.AuthenticateAsync(emailOptions.Username, emailOptions.Password, ect);

                await smtp.SendAsync(email, ect);
            }
            finally
            {
                if (smtp.IsConnected)
                    await smtp.DisconnectAsync(true, CancellationToken.None);
            }
        }, ct);
    }
}