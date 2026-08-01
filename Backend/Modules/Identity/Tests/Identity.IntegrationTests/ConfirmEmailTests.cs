using System.Text;
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
    public async Task ConfirmEmail_WhenEmailIsNotFound_ShouldReturnError()
    {
        //Arrange 
        var faker = new Faker();
        var email = faker.Person.Email;
        const string token = "some-token";

        //Act 
        var response = await Client.ConfirmEmailAsync(email, token);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await response.ShouldBeErrorAsync<EmailOrConfirmationTokenIsNotValidError>();
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
    public async Task ConfirmEmail_WhenTokenIsInvalid_ShouldReturnBadRequest()
    {
        //Arrange 
        var user = Fakers.RegisterRequestFaker.Generate();
        await Client.RegisterUserAsync(user);
        var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("invalid-token"));

        //Act 
        var response = await Client.ConfirmEmailAsync(user.Email, token);

        //Assert 
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await response.ShouldBeErrorAsync<EmailOrConfirmationTokenIsNotValidError>();
    }

    [Fact]
    public async Task ConfirmEmail_WhenTokenIsNotBase64_ShouldReturnBadRequest()
    {
        //Arrange 
        var user = Fakers.RegisterRequestFaker.Generate();
        await Client.RegisterUserAsync(user);
        const string token = "invalid-token";

        //Act 
        var response = await Client.ConfirmEmailAsync(user.Email, token);

        //Assert 
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await response.ShouldBeErrorAsync<EmailOrConfirmationTokenIsNotValidError>();
    }
}