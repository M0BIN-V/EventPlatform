using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Options;

namespace Shared.IntegrationTests.Common;

public class WebApiFactory(string databaseConnectionString, EmailOptions emailOptions) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:event-platform-db", databaseConnectionString);
        builder.ConfigureServices(s => s.AddSingleton(Options.Create(emailOptions)));
    }

}