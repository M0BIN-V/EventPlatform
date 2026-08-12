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

        var organizationExists = await DbContext.OrganizationMembers.AnyAsync();
        organizationExists.ShouldBeFalse();
    }

    [Fact]
    public async Task CreateOrganization_WhenSlugExists_ShouldReturnConflict()
    {
        await AuthenticateClient();

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
}