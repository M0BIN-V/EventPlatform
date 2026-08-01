using Identity.Application.Features.Login;
using Identity.Application.Features.Register;
using Shared.IntegrationTests.Abstractions;
using Shared.IntegrationTests.Fixtures;

namespace Identity.IntegrationTests;

[Collection("Integration")]
public class LoginUserTests(IntegrationTestFixture testFixture) : IntegrationTest(testFixture)
{
    [Fact]
    public async Task LoginUser_WhenPasswordIsInvalid_ShouldReturnUnauthorizedResponse()
    {
        //Arrange 
        const string email = "user@email.com";
        const string password = "pasd#$98Ddd";

        var request = new LoginRequest(email, password);

        await Client.PostAsJsonAsync("api/identity/register",
            new RegisterRequest("user first name", "user last name", email, password));
        
        //TODO 
        
    }
}