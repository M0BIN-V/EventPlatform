using BuildingBlocks.Application.Contracts;
using Organizations.Application.Common.Contracts.Persistence;
using Organizations.Application.Features.CreateOrganization;
using Shared.UnitTests.Abstractions;

namespace Organizations.UnitTests.Application.Features;

public class CreateOrganizationHandlerTests : HandlerTest<CreateOrganizationHandler, CreateOrganizationRequest,
    CreateOrganizationResponse>
{
    private readonly ICurrentUser _currentUser = For<ICurrentUser>();
    private readonly IOrganizationMemberRepository _organizationMemberRepository = For<IOrganizationMemberRepository>();
    private readonly IOrganizationRepository _organizationRepository = For<IOrganizationRepository>();
    private readonly IOrganizationUnitOfWork _unitOfWork = For<IOrganizationUnitOfWork>();

    public CreateOrganizationHandlerTests()
    {
        var validator = For<IValidator<CreateOrganizationRequest>>();

        validator.ValidateAsync(Any<CreateOrganizationRequest>(), Any<CancellationToken>())
            .Returns(new ValidationResult());

        Handler = new CreateOrganizationHandler(
            _organizationRepository,
            _organizationMemberRepository,
            validator,
            _currentUser,
            _unitOfWork);

        Validator = validator;
    }

    protected override CreateOrganizationHandler Handler { get; }

    protected override IValidator<CreateOrganizationRequest> Validator { get; }

    protected override CreateOrganizationRequest GetRequest()
    {
        return new CreateOrganizationRequest(
            "Test Organization",
            "test-organization",
            "This is a test organization.");
    }

    [Fact]
    public async Task Handler_WhenSlugExists_ShouldReturnSlugAlreadyExistsError()
    {
        // Arrange
        var request = GetRequest();

        _organizationRepository.SlugExistsAsync(request.Slug, Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await Handler.HandleAsync(request);

        // Assert
        result.Value.ShouldBeOfType<OrganizationSlugAlreadyExistsError>();
    }

    [Fact]
    public async Task Handler_WhenRequestIsValid_ShouldCreateOrganizationAndReturnResponse()
    {
        // Arrange
        var request = GetRequest();
        var userId = Guid.NewGuid().ToString();

        _currentUser.Id.Returns(userId);
        _organizationRepository.SlugExistsAsync(request.Slug, Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await Handler.HandleAsync(request);

        // Assert
        result.Value.ShouldBeOfType<CreateOrganizationResponseData>();
        var responseData = result.Value as CreateOrganizationResponseData;
        responseData.ShouldNotBeNull();
        responseData.Name.ShouldBe(request.Name);
        responseData.Slug.ShouldBe(request.Slug.ToLowerInvariant());
    }
}