using Files.Application.Common.Errors;
using FluentValidation.Results;

namespace Files.Application.Features.CreateUpload;

public record SuccessCreateUploadResponse(
    string Url,
    DateTime ExpiresAtUtc,
    IReadOnlyDictionary<string, string> RequiredHeaders);

public union CreateUploadResponse(
    SuccessCreateUploadResponse,
    List<ValidationFailure>,
    InvalidOwnerError,
    InvalidPurposeError,
    List<PolicyValidationError>);