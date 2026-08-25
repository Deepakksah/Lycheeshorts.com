using System;
using System.Collections.Generic;
using Lychee.Publisher.Domain.Common;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Domain.Entities;

public sealed class Video : AuditableEntity
{
	public Guid UserId { get; set; }

	public User? User { get; set; }

	public required string SourceType { get; set; }

	public string? SourceUrl { get; set; }

	public required string OriginalFileUri { get; set; }

	public string? ThumbnailUri { get; set; }

	public string? Title { get; set; }

	public TimeSpan? Duration { get; set; }

	public ProcessingStatus Status { get; set; } = ProcessingStatus.Uploaded;

	public ICollection<ShortClip> Shorts { get; set; } = new List<ShortClip>();
}
