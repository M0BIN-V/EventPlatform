using System;
using System.Collections.Generic;

namespace Files.Contracts.Dtos;

public record PresignedUploadResponse(
    string Url,
    string ObjectKey);
