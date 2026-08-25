using System;
using Lychee.Publisher.Domain.Entities;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

internal static class SecuritySeed
{
	public static readonly DateTimeOffset CreatedAtUtc = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);

	public static readonly Guid UserRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");

	public static readonly Guid AdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

	public static readonly Guid ManageVideosPermissionId = Guid.Parse("33333333-3333-3333-3333-333333333333");

	public static readonly Guid ManageSchedulesPermissionId = Guid.Parse("44444444-4444-4444-4444-444444444444");

	public static readonly Guid ManageAdminPermissionId = Guid.Parse("55555555-5555-5555-5555-555555555555");

	public static readonly Permission[] Permissions = new Permission[3]
	{
		new Permission
		{
			Id = ManageVideosPermissionId,
			Name = "videos.manage",
			Description = "Create and process videos.",
			CreatedAtUtc = CreatedAtUtc
		},
		new Permission
		{
			Id = ManageSchedulesPermissionId,
			Name = "schedules.manage",
			Description = "Create and manage publishing schedules.",
			CreatedAtUtc = CreatedAtUtc
		},
		new Permission
		{
			Id = ManageAdminPermissionId,
			Name = "admin.manage",
			Description = "Access administrative features.",
			CreatedAtUtc = CreatedAtUtc
		}
	};

	public static readonly RolePermission[] RolePermissions = new RolePermission[5]
	{
		new RolePermission
		{
			RoleId = UserRoleId,
			PermissionId = ManageVideosPermissionId
		},
		new RolePermission
		{
			RoleId = UserRoleId,
			PermissionId = ManageSchedulesPermissionId
		},
		new RolePermission
		{
			RoleId = AdminRoleId,
			PermissionId = ManageVideosPermissionId
		},
		new RolePermission
		{
			RoleId = AdminRoleId,
			PermissionId = ManageSchedulesPermissionId
		},
		new RolePermission
		{
			RoleId = AdminRoleId,
			PermissionId = ManageAdminPermissionId
		}
	};
}
