using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record ShortClipResponse(Guid Id, Guid VideoId, string OutputUri, string? Title, string? Description, string? Hashtags, string? CaptionsJson, decimal ViralityScore, TimeSpan StartTime, TimeSpan EndTime, string Status, DateTimeOffset CreatedAtUtc);
