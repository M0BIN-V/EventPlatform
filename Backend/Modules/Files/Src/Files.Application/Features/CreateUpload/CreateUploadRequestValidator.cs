using FluentValidation;

namespace Files.Contracts.Features.CreateUpload;

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