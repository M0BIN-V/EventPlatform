using Files.Contracts.Errors;
using FluentValidation.Results;

namespace Files.Contracts.Features.CreateUpload;

public record SuccessCreateUploadResponse(
    Guid Id,
    string Url,
    DateTime ExpiresAtUtc,
    IReadOnlyDictionary<string, string> RequiredHeaders);

public union CreateUploadResponse(
    SuccessCreateUploadResponse,
    List<ValidationFailure>,
    InvalidOwnerError,
    InvalidPurposeError,
    List<PolicyValidationError>);