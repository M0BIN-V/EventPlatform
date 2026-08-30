using System;
using System.Collections.Generic;

namespace Files.Application.Contracts.Dtos;

public record ObjectMetadataDto(IDictionary<string,string>? Metadata, long Size, DateTime LastModified);
