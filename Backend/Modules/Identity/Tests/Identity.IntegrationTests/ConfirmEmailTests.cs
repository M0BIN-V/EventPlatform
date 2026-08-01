using Bogus;
using Messaging;
using Microsoft.AspNetCore.WebUtilities;
using Shared.IntegrationTests.Abstractions;
using Shared.IntegrationTests.Fixtures;
using Shared.IntegrationTests.Helpers;

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
        var response = await Client.ConfirmEmailAsync(email, "invalid-token");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        await response.ShouldBeErrorAsync<UserNotFoundError>();
    }

    [Fact]
    public async Task ConfirmEmail_WhenTokenIsValid_ShouldReturnSuccess()
    {
        //Arrange
        var registerRequest = Fakers.RegisterRequestFaker.Generate();

        var (_, tracked) = await TestFixture.TrackAsync(() => Client.RegisterUserAsync(registerRequest));

        var confirmationUrl = tracked.Sent.SingleMessage<ConfirmEmailRequestedEvent>().ConfirmationUrl;
        var uri = new Uri(confirmationUrl);
        var query = QueryHelpers.ParseQuery(uri.Query);
        var token = query["token"];

        //Act 
        var response = await Client.ConfirmEmailAsync(registerRequest.Email, token!);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ConfirmEmail_WhenTokenIsInvalid_ShouldReturnError()
    {
        //Arrange 
        var user = Fakers.RegisterRequestFaker.Generate();
        await Client.RegisterUserAsync(user);
        const string token = "this-is-invalid-token";

        //Act 
        var response = await Client.GetAsync($"/api/identity/confirm-email?email={user.Email}&token={token}");

        //Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}