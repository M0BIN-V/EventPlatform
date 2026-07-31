using Identity.Application.Features.Register;
using Messaging;

namespace Identity.IntegrationTests;

[Collection("Integration")]
public class RegisterUserTests(IntegrationTestFixture testFixture) : IntegrationTest(testFixture)
{
    private const string Endpoint = "api/identity/register";

    [Fact]
    public async Task RegisterUser_WhenUserAlreadyExists_ShouldReturnConflictResponse()
    {
        //Arrange
        var request = new RegisterRequest
        (
            "user first name ",
            "user last name",
            "user@email.com",
            "asdf(*dsFD_223"
        );

        await Client.PostAsJsonAsync(Endpoint, request);


        //Act
        var (response, tracked) = await TestFixture.TrackAsync(() =>
            Client.PostAsJsonAsync(Endpoint, request));


        //Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        await response.ShouldBeErrorAsync<UserAlreadyExistsError>(
            message: $"User with email '{request.Email}' already exists.");

        tracked.Sent.MessagesOf<ConfirmEmailRequestedEvent>()
            .ShouldBeEmpty();
    }

    [Fact]
    public async Task RegisterUser_WhenEmailIsNotValid_ShouldReturnBadRequestResponse()
    {
        //Arrange
        var request = new RegisterRequest
        (
            "user first name ",
            "user last name",
            "user email.com",
            "asdf(*dsFD_223"
        );


        //Act
        var (response, tracked) = await TestFixture.TrackAsync(() =>
            Client.PostAsJsonAsync(Endpoint, request));

        //Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        tracked.Sent.MessagesOf<ConfirmEmailRequestedEvent>()
            .ShouldBeEmpty();
    }

    [Fact]
    public async Task RegisterUser_WhenUserDoesNotExist_ShouldReturnCreatedResponse()
    {
        //Arrange
        var request = new RegisterRequest
        (
            "user first name ",
            "user last name",
            "user@email.com",
            "asdf(*dsFD_223"
        );


        //Act
        var (response, tracked) = await TestFixture.TrackAsync(() =>
            Client.PostAsJsonAsync(Endpoint, request));

        //assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var publishedEvent = tracked.Sent.SingleMessage<ConfirmEmailRequestedEvent>();
        publishedEvent.ShouldNotBeNull();
        publishedEvent.Email.ShouldBe(request.Email);
    }
}