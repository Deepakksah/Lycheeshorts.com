using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.ToTable("Roles");
		builder.HasKey((Role role) => role.Id);
		builder.Property((Role role) => role.Name).HasMaxLength(64).IsRequired();
		builder.HasIndex((Role role) => role.Name).IsUnique();
		builder.HasData(new Role
		{
			Id = SecuritySeed.UserRoleId,
			Name = "User",
			CreatedAtUtc = SecuritySeed.CreatedAtUtc
		}, new Role
		{
			Id = SecuritySeed.AdminRoleId,
			Name = "Admin",
			CreatedAtUtc = SecuritySeed.CreatedAtUtc
		});
	}
}
