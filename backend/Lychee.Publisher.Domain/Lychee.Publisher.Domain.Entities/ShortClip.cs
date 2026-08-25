using System;
using System.Collections.Generic;
using Lychee.Publisher.Domain.Common;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Domain.Entities;

public sealed class ShortClip : AuditableEntity
{
	public Guid VideoId { get; set; }

	public Video? Video { get; set; }

	public required string OutputUri { get; set; }

	public string? Title { get; set; }

	public string? Description { get; set; }

	public string? Hashtags { get; set; }

	public string? CaptionsJson { get; set; }

	public decimal ViralityScore { get; set; }

	public TimeSpan StartTime { get; set; }

	public TimeSpan EndTime { get; set; }

	public ProcessingStatus Status { get; set; } = ProcessingStatus.Processed;

	public ICollection<PublishingSchedule> Schedules { get; set; } = new List<PublishingSchedule>();
}
