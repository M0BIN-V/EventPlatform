using Microsoft.Extensions.Hosting;

namespace AppHost.Extensions;

public static class MailPitExtensions
{
    public static IResourceBuilder<ProjectResource> ConfigureMailSettings(
        this IResourceBuilder<ProjectResource> api,
        IResourceBuilder<MailPitContainerResource> mailpit)
    {
        return api.WithEnvironment(context =>
        {
            var smtp = mailpit.GetEndpoint("smtp");

            context.EnvironmentVariables["EmailSettings__SmtpServer"] = smtp.Host;
            context.EnvironmentVariables["EmailSettings__Port"] = smtp.Property(EndpointProperty.Port);
            context.EnvironmentVariables["EmailSettings__Username"] = "";
            context.EnvironmentVariables["EmailSettings__Password"] = "";
            context.EnvironmentVariables["EmailSettings__EnableSsl"] = "false";
            context.EnvironmentVariables["EmailSettings__DefaultFromEmail"] = "noreply@eventplatform.local";
            context.EnvironmentVariables["EmailSettings__DefaultFromName"] = "Event Platform";
            context.EnvironmentVariables["EmailSettings__Security"] = "Auto";
        });
    }
}