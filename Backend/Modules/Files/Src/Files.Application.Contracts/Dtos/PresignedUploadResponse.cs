using System;
using System.Collections.Generic;

namespace Files.Application.Contracts.Dtos;

public record PresignedUploadResponse(string Url, IDictionary<string,string>? Fields, DateTime ExpiresAt, string ObjectKey);
