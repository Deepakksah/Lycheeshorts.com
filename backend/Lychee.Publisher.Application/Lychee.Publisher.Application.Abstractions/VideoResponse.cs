using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record VideoResponse(Guid Id, Guid UserId, string SourceType, string? SourceUrl, string OriginalFileUri, string? ThumbnailUri, string? Title, TimeSpan? Duration, string Status, DateTimeOffset CreatedAtUtc);
