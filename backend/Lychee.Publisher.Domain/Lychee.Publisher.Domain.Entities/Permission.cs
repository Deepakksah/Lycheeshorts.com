using System;
using System.Collections.Generic;

namespace Lychee.Publisher.Domain.Entities;

public sealed class Permission
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public required string Name { get; set; }

	public string? Description { get; set; }

	public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

	public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
