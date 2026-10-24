using Files.Application.Common.Errors;
using Files.Domain.Constants;
using FluentValidation.Results;
using OneOf;

namespace Files.Application.Features.CompleteUpload;

public class CompleteUploadHandler(
    IFilesRepository repository,
    IFilesUnitOfWork unitOfWork,
    IObjectStorageService objectStorage,
    IValidator<CompleteUploadRequest> validator)
    : Handler<CompleteUploadRequest, CompleteUploadResponse>
{
    public override async Task<CompleteUploadResponse> HandleAsync(
        CompleteUploadRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid) return validationResult.Errors;

        var file = await repository.GetByIdAsync(request.FileId, ct);

        if (file is null || file.Status != FileStatus.Pending)
            return new InvalidFileStatusError();

        if (await objectStorage.ObjectExistsAsync(file.ObjectKey!))
            return new FileNotFoundError();
     
        throw new NotImplementedException("CompleteUploadHandler.HandleAsync is not implemented yet.");
        // file.MarkUploaded();
    }
}

public record CompleteUploadRequest(Guid FileId);

public class CompleteUploadRequestValidator : AbstractValidator<CompleteUploadRequest>
{
    public CompleteUploadRequestValidator()
    {
        RuleFor(x => x.FileId).NotEmpty();
    }
}

[GenerateOneOf]
public partial class CompleteUploadResponse : OneOfBase<
    string,
    List<ValidationFailure>,
    InvalidFileStatusError,
    FileNotFoundError>;