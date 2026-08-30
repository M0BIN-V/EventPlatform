namespace Files.Application.Features.CreateUpload;

public class CreateUploadRequestValidator : AbstractValidator<CreateUploadRequest>
{
    private const long MaxOrganizationLogoBytes = 5 * 1024 * 1024; // 5 MB

    public CreateUploadRequestValidator()
    {
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Purpose).NotEmpty();
        RuleFor(x => x.MinLength).NotNull();
        RuleFor(x => x.MaxLength).NotNull();
        RuleFor(x => x.Purpose).IsInEnum();
    }
}