namespace Files.Application.Common.Errors;

public record InvalidFileStatusError()
    : Error(nameof(InvalidFileStatusError), "The file is not in a valid status for this operation.");

public record FileNotFoundError() : Error(nameof(FileNotFoundError), "The file was not found in the object storage.");