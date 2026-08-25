using System;
using Lychee.Publisher.Domain.Common;

namespace Lychee.Publisher.Domain.Entities;

public sealed class RefreshToken : AuditableEntity
{
	public Guid UserId { get; set; }

	public User? User { get; set; }

	public required string TokenHash { get; set; }

	public DateTimeOffset ExpiresAtUtc { get; set; }

	public DateTimeOffset? RevokedAtUtc { get; set; }

	public string? ReplacedByTokenHash { get; set; }

	public string? CreatedByIp { get; set; }

	public string? RevokedByIp { get; set; }

	public bool IsActive(DateTimeOffset utcNow)
	{
		return !RevokedAtUtc.HasValue && ExpiresAtUtc > utcNow;
	}
}
