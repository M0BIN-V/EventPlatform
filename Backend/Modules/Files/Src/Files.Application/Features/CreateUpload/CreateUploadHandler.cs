using BuildingBlocks.Domain.Contracts;
using Files.Application.Common.Contracts.Persistence;
using Files.Application.Common.Contracts.Services;
using Files.Contracts.Common.Enums;

namespace Files.Application.Features.CreateUpload;

public class CreateUploadHandler(
    IFilesRepository filesRepo,
    ITimeProvider timeProvider,
    IFilesUnitOfWork unitOfWork,
    IObjectStorageService objectStorage,
    IValidator<CreateUploadRequest> validator)
    : Handler<CreateUploadRequest, CreateUploadResponse>
{
    private static readonly TimeSpan PresignExpiry = TimeSpan.FromMinutes(15);

    public override async Task<CreateUploadResponse> HandleAsync(CreateUploadRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var now = timeProvider.Now;

        var file = File.CreatePending(FilePurpose.OrganizationProfilePicture, now);

        await filesRepo.AddAsync(file, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var uploadRequest = await objectStorage.CreatePresignedUploadAsync(
            file.Id.ToString(),
            timeProvider.Now.Add(PresignExpiry),
            request.Purpose,
            request.MinLength,
            request.MaxLength);


        return new CreateUploadSuccess(file.Id, uploadRequest.Url);
    }
}