using Files.Application.Common.Contracts.Services;
using Files.Application.Common.Errors;
using FluentValidation.Results;

namespace Files.Application.Features.CreateUpload;

public record SuccessCreateUploadResponse;

public union CreateUploadResponse(
    SuccessCreateUploadResponse,
    List<ValidationFailure>,
    InvalidOwnerError,
    InvalidPurposeError);

public class CreateUploadHandler(
    IOwnerService ownerService,
    IValidator<CreateUploadRequest> validator) : Handler<CreateUploadRequest, CreateUploadResponse>
{
    public override async Task<CreateUploadResponse> HandleAsync(CreateUploadRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var owner = ownerService.GetOwner(request.OwnerId);
        if (owner is null) return new InvalidOwnerError();

        var purpose = owner.Purposes.SingleOrDefault(p => p.Id == request.PurposeId);
        if (purpose is null) return new InvalidPurposeError();
    }
}