using Files.Application.Common.Contracts.Services;
using Files.Contracts.Errors;
using Files.Contracts.Features.CreateUpload;
using Files.Domain.Entities;
using Files.Domain.Enums;

namespace Files.Application.Features.CreateUpload;

public class CreateUploadHandler(
    TimeProvider timeProvider,
    IStorageService storage,
    IFilesRepository repo,
    IFilesUnitOfWork unitOfWork,
    IOwnerService ownerService,
    IValidator<CreateUploadRequest> validator) : Handler<CreateUploadRequest, CreateUploadResponse>
{
    public override async Task<CreateUploadResponse> HandleAsync(CreateUploadRequest request,
        CancellationToken ct = default)
    {
        // validate request 
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        // validate owner 
        var owner = ownerService.GetOwner(request.OwnerId);
        if (owner is null) return new InvalidOwnerError();

        // validate purpose 
        var purpose = owner.Purposes.SingleOrDefault(p => p.Id == request.PurposeId);
        if (purpose is null) return new InvalidPurposeError();

        // validate validation policies 
        var policyErrors = ValidatePolicies(purpose.Policy, request);
        if (policyErrors.Count != 0) return policyErrors;

        var file = new File
        {
            Id = Guid.CreateVersion7(),
            ObjectKey = Guid.CreateVersion7().ToString(),
            ContentType = request.ContentType,
            FileName = request.FileName,
            Size = request.Size,
            IsPrivate = purpose.Policy.IsPrivate,
            OwnerId = request.OwnerId,
            PurposeId = request.PurposeId,
            Status = FileStatus.Pending,
            CreatedAt = timeProvider.GetUtcNow(),
            UploadedAt = default,
            ReadyAt = null
        };

        var target = await storage.CreateUploadTargetAsync(file.ObjectKey, file.ContentType, file.Size,
            TimeSpan.FromMinutes(5));

        await repo.AddAsync(file, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new SuccessCreateUploadResponse(file.Id, target.Url, target.ExpiresAtUtc, target.RequiredHeaders);
    }

    private static List<PolicyValidationError> ValidatePolicies(FilePolicy policy, CreateUploadRequest request)
    {
        var errors = new List<PolicyValidationError>();

        if (policy.MaxSize >= request.Size && request.Size >= policy.MinSize)
            errors.Add(new PolicyValidationError("The file size is not within the allowed range."));

        if (!policy.ValidContentTypes.Contains(request.ContentType))
            errors.Add(new PolicyValidationError("The file content type is not allowed."));


        return errors;
    }
}