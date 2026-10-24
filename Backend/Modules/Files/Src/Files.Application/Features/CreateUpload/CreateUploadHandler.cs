namespace Files.Application.Features.CreateUpload;

public class CreateUploadHandler(
    IFilesRepository filesRepo,
    TimeProvider timeProvider,
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

        var now = timeProvider.GetUtcNow();

        throw new NotImplementedException();
        // var file = File.CreatePending(request.ModuleName, now);
        //
        // await filesRepo.AddAsync(file, ct);
        // await unitOfWork.SaveChangesAsync(ct);
        //
        // var objectKey = $"{request.ModuleName}/{string.Join('/', request.PathSegments)}/{file.Id}_{request.FileName}";
        //
        // var uploadRequest = await objectStorage.CreatePresignedUploadAsync(
        //     objectKey,
        //     now.Add(PresignExpiry),
        //     request.MinLength,
        //     request.MaxLength);
        //
        //
        // return new CreateUploadSuccess(file.Id, uploadRequest.Url);
    }
}