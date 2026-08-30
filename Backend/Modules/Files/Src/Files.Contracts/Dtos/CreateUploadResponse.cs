using FluentValidation.Results;
using OneOf;

namespace Files.Contracts.Dtos;

public record CreateUploadSuccess(Guid FileId, string Url);

[GenerateOneOf]
public partial class CreateUploadResponse : OneOfBase<
    CreateUploadSuccess,
    List<ValidationFailure>>;