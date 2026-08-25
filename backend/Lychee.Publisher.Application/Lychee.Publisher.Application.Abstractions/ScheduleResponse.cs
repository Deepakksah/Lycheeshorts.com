using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record ScheduleResponse(Guid Id, Guid ShortClipId, Guid SocialAccountId, string Platform, DateTimeOffset PublishAtUtc, string Status, string? ExternalPostId, string? FailureReason, DateTimeOffset CreatedAtUtc);
