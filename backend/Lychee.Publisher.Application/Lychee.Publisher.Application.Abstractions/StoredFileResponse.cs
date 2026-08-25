namespace Lychee.Publisher.Application.Abstractions;

public sealed record StoredFileResponse(string Uri, string FileName, string? ContentType, long SizeBytes);
