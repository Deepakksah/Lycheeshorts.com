using System;

namespace Lychee.Publisher.Domain.Common;

public abstract class AuditableEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

	public DateTimeOffset? UpdatedAtUtc { get; set; }

	public string? CreatedByUserId { get; set; }

	public string? UpdatedByUserId { get; set; }
}
