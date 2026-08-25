using System;
using Lychee.Publisher.Domain.Common;

namespace Lychee.Publisher.Domain.Entities;

public sealed class PasswordResetToken : AuditableEntity
{
	public Guid UserId { get; set; }

	public required string TokenHash { get; set; }

	public DateTimeOffset ExpiresAtUtc { get; set; }

	public DateTimeOffset? UsedAtUtc { get; set; }

	public string? CreatedByIp { get; set; }

	public User? User { get; set; }

	public bool IsActive(DateTimeOffset now)
	{
		return !UsedAtUtc.HasValue && ExpiresAtUtc > now;
	}
}
