using System.Net.Http.Json;
using Identity.Application.Features.Register;

namespace Shared.IntegrationTests.Extensions;


public static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        public async Task<HttpResponseMessage> ConfirmEmailAsync(string email, string token)
        {
            return await client.GetAsync($"/api/identity/confirm-email?email={email}&token={token}");
        }

        public Task<HttpResponseMessage> RegisterUserAsync(RegisterRequest request)
        {
            return client.PostAsJsonAsync("/api/identity/register", request);
        }
    }
}