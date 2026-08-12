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
}