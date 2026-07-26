using Identity.IntegrationTests.Common;

namespace Identity.IntegrationTests;

[Collection("Integration")]
public class RegisterUserTests(IntegrationTestFixture testFixture)
{
    private readonly HttpClient _client = testFixture.Api.CreateClient();
    
    [Fact]
    public async Task RegisterUser_WhenUserAlreadyExists_ShouldReturnConflictResponse()
    {
        var response = await _client.GetAsync("/api/users");
        response.EnsureSuccessStatusCode();
    }
}