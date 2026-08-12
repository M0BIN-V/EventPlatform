using System.Net.Http.Json;
using Identity.Application.Features.Login;
using Identity.Application.Features.Register;
using Organizations.Application.Features.CreateOrganization;

namespace Shared.IntegrationTests.Extensions;

public static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        public async Task<HttpResponseMessage> CreateOrganizationAsync(CreateOrganizationRequest request)
        {
            return await client.PostAsJsonAsync("/api/organizations", request);
        }

        public async Task<HttpResponseMessage> ConfirmEmailAsync(string email, string token)
        {
            return await client.GetAsync($"/api/identity/confirm-email?email={email}&token={token}");
        }

        public Task<HttpResponseMessage> RegisterUserAsync(RegisterRequest request)
        {
            return client.PostAsJsonAsync("/api/identity/register", request);
        }

        public Task<HttpResponseMessage> LoginAsync(LoginRequest request)
        {
            return client.PostAsJsonAsync("/api/identity/login", request);
        }
    }
}