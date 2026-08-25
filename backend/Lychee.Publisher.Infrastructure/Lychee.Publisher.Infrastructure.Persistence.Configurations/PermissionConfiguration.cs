using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
	public void Configure(EntityTypeBuilder<Permission> builder)
	{
		builder.ToTable("Permissions");
		builder.HasKey((Permission permission) => permission.Id);
		builder.Property((Permission permission) => permission.Name).HasMaxLength(120).IsRequired();
		builder.Property((Permission permission) => permission.Description).HasMaxLength(280);
		builder.HasIndex((Permission permission) => permission.Name).IsUnique();
		builder.HasData(SecuritySeed.Permissions);
	}
}
