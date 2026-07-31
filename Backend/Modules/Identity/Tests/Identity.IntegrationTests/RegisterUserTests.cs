using Identity.Application.Features.Register;

namespace Identity.IntegrationTests;

[Collection("Integration")]
public class RegisterUserTests(IntegrationTestFixture testFixture) : IAsyncLifetime
{
    private const string Endpoint = "api/identity/register";
    private readonly HttpClient _client = testFixture.CreateClient();

    public async Task InitializeAsync()
    {
        await testFixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

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

        await _client.PostAsJsonAsync(Endpoint, request);


        //Act
        var response = await _client.PostAsJsonAsync(Endpoint, request);


        //Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        await response.ShouldBeErrorAsync<UserNotFoundError>(
            message: $"User with email '{request.Email}' already exists.");
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
        var response = await _client.PostAsJsonAsync(Endpoint, request);


        //Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
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
        var response = await _client.PostAsJsonAsync(Endpoint, request);


        //Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task RegisterUser_WhenUserDoesNotExist_ShouldReturnCreatedRespons2()
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
        var response = await _client.PostAsJsonAsync(Endpoint, request);


        //Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task RegisterUser_WhenUserDoesNotExist_ShouldReturnCreatedRespons3()
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
        var response = await _client.PostAsJsonAsync(Endpoint, request);


        //Assert 
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}