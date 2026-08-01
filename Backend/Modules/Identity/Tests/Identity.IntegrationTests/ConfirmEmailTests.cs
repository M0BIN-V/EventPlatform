using Bogus;
using Identity.Application.Features.Register;
using Messaging;
using Microsoft.AspNetCore.WebUtilities;

namespace Identity.IntegrationTests;

[Collection("Integration")]
public class ConfirmEmailTests(IntegrationTestFixture testFixture) : IntegrationTest(testFixture)
{
    [Fact]
    public async Task ConfirmEmail_WhenEmailIsNotFound_ShouldReturnNotFound()
    {
        //Arrange 
        var faker = new Faker();
        var email = faker.Person.Email;

        //Act 
        var response = await Client.GetAsync($"/api/identity/confirm-email?email={email}&token=invalid-token");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await response.ShouldBeErrorAsync<UserNotFoundError>();
    }

    [Fact]
    public async Task ConfirmEmail_WhenTokenIsValid_ShouldReturnSuccess()
    {
        //Arrange 
        var faker = new Faker();

        var firstName = faker.Person.FirstName;
        var lastName = faker.Person.LastName;
        var email = faker.Person.Email;
        const string password = "dsafasdf#$)(54D";

        var registerRequest = new RegisterRequest(firstName, lastName, email, password);

        var (_, tracked) = await TestFixture.TrackAsync(() =>
            Client.PostAsJsonAsync("/api/identity/register", registerRequest));

        var confirmationUrl = tracked.Sent.SingleMessage<ConfirmEmailRequestedEvent>().ConfirmationUrl;
        var uri = new Uri(confirmationUrl);
        var query = QueryHelpers.ParseQuery(uri.Query);
        var token = query["token"];
        
        //Act 
        var response = await Client.GetAsync($"/api/identity/confirm-email?email={email}&token={token}");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}