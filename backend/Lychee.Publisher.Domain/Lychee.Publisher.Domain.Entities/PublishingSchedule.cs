using System;
using Lychee.Publisher.Domain.Common;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Domain.Entities;

public sealed class PublishingSchedule : AuditableEntity
{
	public Guid ShortClipId { get; set; }

	public ShortClip? ShortClip { get; set; }

	public Guid SocialAccountId { get; set; }

	public SocialAccount? SocialAccount { get; set; }

	public PlatformType Platform { get; set; }

	public DateTimeOffset PublishAtUtc { get; set; }

	public ProcessingStatus Status { get; set; } = ProcessingStatus.Scheduled;

	public string? ExternalPostId { get; set; }

	public string? FailureReason { get; set; }
}
