using System;
using Lychee.Publisher.Domain.Common;

namespace Lychee.Publisher.Domain.Entities;

public sealed class UserSession : AuditableEntity
{
	public Guid UserId { get; set; }

	public string? IpAddress { get; set; }

	public string? UserAgent { get; set; }

	public DateTimeOffset LastSeenAtUtc { get; set; } = DateTimeOffset.UtcNow;

	public DateTimeOffset? RevokedAtUtc { get; set; }

	public User? User { get; set; }
}
