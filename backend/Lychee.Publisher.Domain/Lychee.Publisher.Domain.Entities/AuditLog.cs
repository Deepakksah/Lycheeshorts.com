using System;
using Lychee.Publisher.Domain.Common;

namespace Lychee.Publisher.Domain.Entities;

public sealed class AuditLog : AuditableEntity
{
	public Guid? UserId { get; set; }

	public required string Action { get; set; }

	public string? EntityName { get; set; }

	public string? EntityId { get; set; }

	public string? MetadataJson { get; set; }
}
