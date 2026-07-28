using Shared.IntegrationTests;
using Shared.IntegrationTests.Common;

namespace Identity.IntegrationTests;

[Collection("Integration")]
public class RegisterUserTests(IntegrationTestFixture testFixture)
{
    private readonly ApiClient _client = new(testFixture.Api.CreateClient());

    [Fact]
    public async Task RegisterUser_WhenUserAlreadyExists_ShouldReturnConflictResponse()
    {
        var request = new RegisterRequest
        {
            FirstName = "user first name ",
            LastName = "user last name",
            Email = "user email ",
            Password = "asdf_09sdfKd2$"
        };

        await _client.RegisterUserAsync(request);

        await _client.RegisterUserAsync(request);
    }
}