using BuildingBlocks.Application;

namespace Files.Contracts.Errors;

public record InvalidFileStatusError()
    : Error(nameof(InvalidFileStatusError), "The file is not in a valid status for this operation.");

public record FileNotFoundError() : Error(nameof(FileNotFoundError), "The file was not found in the object storage.");

public record InvalidOwnerError() : Error(nameof(InvalidOwnerError), "The owner is not valid for this operation.");

public record InvalidPurposeError() : Error(nameof(InvalidPurposeError),
    "The purpose is not valid for this operation.");

public record PolicyValidationError(string Message) : Error(nameof(PolicyValidationError),
    "The file does not meet the validation policies for this operation.");