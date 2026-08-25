using System;
using System.Collections.Generic;
using Lychee.Publisher.Domain.Common;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Domain.Entities;

public sealed class SocialAccount : AuditableEntity
{
	public Guid UserId { get; set; }

	public User? User { get; set; }

	public PlatformType Platform { get; set; }

	public required string ExternalAccountId { get; set; }

	public required string DisplayName { get; set; }

	public string? ChannelName { get; set; }

	public bool IsActive { get; set; } = true;

	public ICollection<PublishingSchedule> Schedules { get; set; } = new List<PublishingSchedule>();
}
