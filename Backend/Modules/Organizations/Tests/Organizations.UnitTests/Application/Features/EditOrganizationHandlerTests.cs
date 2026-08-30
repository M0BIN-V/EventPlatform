using BuildingBlocks.Application.Contracts;
using NSubstitute.ReturnsExtensions;
using Organizations.Application.Common.Contracts.Persistence;
using Organizations.Application.Features.EditOrganization;
using Organizations.Domain.Constants;
using Shared.UnitTests.Abstractions;

namespace Organizations.UnitTests.Application.Features;

public class
    EditOrganizationHandlerTests : HandlerTest<EditOrganizationHandler, EditOrganizationRequest,
    EditOrganizationResponse>
{
    private readonly ICurrentUser _currentUser = For<ICurrentUser>();
    private readonly IOrganizationMemberRepository _organizationMemberRepository = For<IOrganizationMemberRepository>();
    private readonly IOrganizationRepository _organizationRepository = For<IOrganizationRepository>();
    private readonly IOrganizationUnitOfWork _unitOfWork = For<IOrganizationUnitOfWork>();

    public EditOrganizationHandlerTests()
    {
        var validator = For<IValidator<EditOrganizationRequest>>();

        validator.ValidateAsync(Any<EditOrganizationRequest>(), Any<CancellationToken>())
            .Returns(new ValidationResult());

        Handler = new EditOrganizationHandler(
            _organizationRepository,
            validator,
            _currentUser,
            _organizationMemberRepository,
            _unitOfWork);

        Validator = validator;
    }

    protected override EditOrganizationHandler Handler { get; }

    protected override IValidator<EditOrganizationRequest> Validator { get; }

    protected override EditOrganizationRequest GetRequest()
    {
        return new EditOrganizationRequest(
            "this-is-old-slug",
            "new name",
            "new-slug",
            "this is new description");
    }

    [Fact]
    public async Task Handler_WhenOrganizationNotExists_ShouldReturnNotFoundError()
    {
        // Arrange
        var request = GetRequest();

        _organizationRepository.GetBySlugAsync(request.Slug, Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await Handler.HandleAsync(request);

        // Assert
        result.Value.ShouldBeOfType<OrganizationNotFoundError>();
    }

    [Fact]
    public async Task Handler_WhenUserIsNotMemberOfOrganization_ShouldReturnUnauthorizedError()
    {
        //Arrange 
        var request = GetRequest();
        var organization =
            new Organization("this is name", "this-is-slug", "this is new description", "this-is-owner-id");
        _organizationRepository.GetBySlugAsync(request.Slug, Any<CancellationToken>())
            .Returns(organization);

        _currentUser.Id.Returns(Guid.NewGuid().ToString());
        _organizationMemberRepository
            .GetByOrganizationAndUserAsync(organization.Id, _currentUser.Id, Any<CancellationToken>())
            .ReturnsNull();

        //Act 
        var result = await Handler.HandleAsync(request);

        //Assert 
        result.Value.ShouldBeOfType<OrganizationUnauthorizedError>();
    }

    [Fact]
    public async Task Handler_ShouldReturnEditedValues()
    {
        //Arrange
        var request = GetRequest();
        const string ownerId = "this-is-owner-id";

        _organizationRepository.GetBySlugAsync(request.Slug, Any<CancellationToken>())
            .Returns(new Organization("this is name", request.Slug, "this is new description", ownerId));

        _organizationMemberRepository
            .GetByOrganizationAndUserAsync(Any<Guid>(), ownerId, Any<CancellationToken>())
            .Returns(new OrganizationMemberShip
            {
                OrganizationId = Guid.NewGuid(),
                UserId = ownerId,
                Role = OrganizationRole.Owner
            });

        _currentUser.Id.Returns(ownerId);

        //Act
        var result = await Handler.HandleAsync(request);

        //Assert 
        var resultValue = result.Value as ViewEditedOrganization;

        resultValue.ShouldNotBeNull();
        resultValue.Slug.ShouldBe(request.NewSlug);
        resultValue.Name.ShouldBe(request.NewName);
        resultValue.Description.ShouldBe(request.NewDescription);
    }


    [Fact]
    public async Task Handler_WhenUserIsNotOwner_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var request = GetRequest();
        const string ownerId = "this-is-owner-id";
        var organization = new Organization("this is name", "this-is-slug", "this is new description", ownerId);

        _organizationRepository.GetBySlugAsync(request.Slug, Any<CancellationToken>())
            .Returns(organization);

        const string invalidUserId = "invalid-user-id";

        _currentUser.Id.Returns(invalidUserId);

        _organizationMemberRepository
            .GetByOrganizationAndUserAsync(organization.Id, invalidUserId, Any<CancellationToken>())
            .Returns(new OrganizationMemberShip
            {
                OrganizationId = organization.Id,
                UserId = invalidUserId,
                Role = OrganizationRole.Member
            });


        // Act
        var result = await Handler.HandleAsync(request);

        // Assert
        result.Value.ShouldBeOfType<OrganizationUnauthorizedError>();
    }
}