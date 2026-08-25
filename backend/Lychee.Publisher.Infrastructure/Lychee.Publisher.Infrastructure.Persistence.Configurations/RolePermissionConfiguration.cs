using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
	public void Configure(EntityTypeBuilder<RolePermission> builder)
	{
		builder.ToTable("RolePermissions");
		builder.HasKey((RolePermission rolePermission) => new { rolePermission.RoleId, rolePermission.PermissionId });
		builder.HasOne((RolePermission rolePermission) => rolePermission.Role).WithMany((Role role) => role.RolePermissions).HasForeignKey((RolePermission rolePermission) => rolePermission.RoleId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasOne((RolePermission rolePermission) => rolePermission.Permission).WithMany((Permission permission) => permission.RolePermissions).HasForeignKey((RolePermission rolePermission) => rolePermission.PermissionId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasData(SecuritySeed.RolePermissions);
	}
}
