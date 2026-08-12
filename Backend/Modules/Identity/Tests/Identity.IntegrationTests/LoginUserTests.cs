using Identity.Application.Features.Login;
using Messaging;
using Microsoft.AspNetCore.WebUtilities;
using Shared.IntegrationTests.Abstractions;
using Shared.IntegrationTests.Fixtures;
using Shared.IntegrationTests.Helpers;

namespace Identity.IntegrationTests;

[Collection("Integration")]
public class LoginUserTests(IntegrationTestFixture testFixture) : IntegrationTest(testFixture)
{
    [Fact]
    public async Task LoginUser_WhenEmailIsNotConfirmed_ShouldReturnForbidden()
    {
        // Arrange
        var registerRequest = Fakers.RegisterRequestFaker.Generate();
        await Client.RegisterUserAsync(registerRequest);

        var request = new LoginRequest(registerRequest.Email, registerRequest.Password);

        // Act
        var response = await Client.LoginAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        await response.ShouldBeErrorAsync<EmailNotConfirmedError>();
    }

    [Fact]
    public async Task LoginUser_WhenPasswordIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = Fakers.RegisterRequestFaker.Generate();

        var (_, tracked) = await TestFixture.TrackAsync(() => Client.RegisterUserAsync(registerRequest));

        var confirmationUrl = tracked.Sent.SingleMessage<ConfirmEmailRequestedEvent>().ConfirmationUrl;
        var uri = new Uri(confirmationUrl);
        var query = QueryHelpers.ParseQuery(uri.Query);
        var token = query["token"];

        await Client.ConfirmEmailAsync(registerRequest.Email, token!);

        var request = new LoginRequest(registerRequest.Email, "invalid-password");

        // Act
        var response = await Client.LoginAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        await response.ShouldBeErrorAsync<InvalidPasswordError>();
    }
}