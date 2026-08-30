using System;
namespace Files.Application.Contracts.Dtos;

public record CreateUploadRequest(
    string OwnerUserId,
    Guid? OrganizationId,
    string FileName,
    string ContentType,
    long? ExpectedSize,
    string? Purpose);
