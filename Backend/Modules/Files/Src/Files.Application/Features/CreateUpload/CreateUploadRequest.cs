namespace Files.Application.Features.CreateUpload;

public record CreateUploadRequest(
    string OwnerId,
    string PurposeId,
    string FileName,
    string ContentType,
    long Size);

public class CreateUploadRequestValidator : AbstractValidator<CreateUploadRequest>
{
    public CreateUploadRequestValidator()
    {
        RuleFor(r => r.OwnerId)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256);

        RuleFor(r => r.PurposeId)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256);

        RuleFor(r => r.FileName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256);

        RuleFor(r => r.ContentType)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256);

        RuleFor(r => r.Size)
            .GreaterThan(0);
    }
}