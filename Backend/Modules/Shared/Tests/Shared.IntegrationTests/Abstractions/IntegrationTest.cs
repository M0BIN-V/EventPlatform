using System.Net.Http.Headers;
using Identity.Application.Common.Contracts.Services;
using Identity.Domain.Constants;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.IntegrationTests.Fixtures;

namespace Shared.IntegrationTests.Abstractions;

public abstract class IntegrationTest<TDbContext>(IntegrationTestFixture testFixture) : IntegrationTest(testFixture)
    where TDbContext : DbContext
{
    private AsyncServiceScope _scope;
    protected TDbContext DbContext { get; private set; } = null!;


    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _scope = TestFixture.WebApiFactory.Services.CreateAsyncScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<TDbContext>();
    }


    public override async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
        await base.DisposeAsync();
    }
}

public abstract class IntegrationTest(IntegrationTestFixture testFixture) : IAsyncLifetime
{
    protected readonly HttpClient Client = testFixture.CreateClient();
    protected readonly IntegrationTestFixture TestFixture = testFixture;

    public virtual async Task InitializeAsync()
    {
        await TestFixture.ResetDatabaseAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task AuthenticateClient(
        string firstName = "user1",
        string lastName = "user1",
        string email = "user1@email.com",
        string password = "uS34__34sdfdD2@")
    {
        await using var scope = TestFixture.WebApiFactory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            EmailConfirmed = true,
            UserName = Guid.CreateVersion7().ToString()
        };

        var userManager = services.GetRequiredService<UserManager<User>>();

        var createUserResult = await userManager.CreateAsync(user, password);

        if (!createUserResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                createUserResult.Errors.Select(x => x.Description));

            throw new InvalidOperationException(
                $"Failed to create user: {errors}");
        }

        const string userRole = Roles.User;
        var addToRoleResult = await userManager.AddToRoleAsync(user, userRole);

        if (!addToRoleResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                createUserResult.Errors.Select(x => x.Description));

            throw new InvalidOperationException(
                $"Failed to create user: {errors}");
        }

        var accessTokenGenerator = services.GetRequiredService<IAccessTokenService>();
        var accessToken = accessTokenGenerator.GenerateAccessToken(user, [userRole]);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}