using Microsoft.EntityFrameworkCore;
using Organizations.Application.Features.CreateOrganization;
using Organizations.Infrastructure.Persistence.DbContext;
using Shared.IntegrationTests.Abstractions;
using Shared.IntegrationTests.Fixtures;

namespace Organizations.IntegrationTests;

[Collection("Integration")]
public class CreateOrganizationTests(IntegrationTestFixture testFixture)
    : IntegrationTest<EfOrganizationDbContext>(testFixture)
{
    [Fact]
    public async Task CreateOrganization_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
    {
        //Arrange
        var request = new CreateOrganizationRequest(
            "Test Organization",
            "This is a test organization",
            "test-organization"
        );

        //Act
        var response = await Client.CreateOrganizationAsync(request);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var organizationExists = await DbContext.OrganizationMemberShips.AnyAsync();
        organizationExists.ShouldBeFalse();
    }

    [Fact]
    public async Task CreateOrganization_WhenSlugExists_ShouldReturnConflict()
    {
        await AuthenticateClient(Client);

        var request = new CreateOrganizationRequest(
            "Test Organization",
            "test-organization",
            "This is  a test organization"
        );

        await Client.CreateOrganizationAsync(request);

        // Act
        var response = await Client.CreateOrganizationAsync(request);

        // Assert
        await response.ShouldBeErrorAsync<OrganizationSlugAlreadyExistsError>();
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var organizationsCount = await DbContext.Organizations.CountAsync();
        organizationsCount.ShouldBe(1);
    }

    [Fact]
    public async Task CreateOrganization_WhenSlugIsValid_ShouldReturnCreated()
    {
        var user = await AuthenticateClient(Client);

        var request = new CreateOrganizationRequest(
            "Test Organization",
            "test-organization",
            "This is  a test organization"
        );

        // Act
        var response = await Client.CreateOrganizationAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var storedOrganization = await DbContext.Organizations.Include(o => o.Members).SingleAsync();
        storedOrganization.Name.ShouldBe(request.Name);
        storedOrganization.Slug.ShouldBe(request.Slug);
        storedOrganization.Description.ShouldBe(request.Description);

        storedOrganization.Members.Single().UserId.ShouldBe(user.Id);
    }
}