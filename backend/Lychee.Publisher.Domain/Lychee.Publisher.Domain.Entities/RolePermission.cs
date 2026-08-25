using System;

namespace Lychee.Publisher.Domain.Entities;

public sealed class RolePermission
{
	public Guid RoleId { get; set; }

	public Guid PermissionId { get; set; }

	public Role? Role { get; set; }

	public Permission? Permission { get; set; }
}
