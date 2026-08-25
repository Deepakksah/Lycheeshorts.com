using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");
		builder.HasKey((User user) => user.Id);
		builder.Property((User user) => user.Email).HasMaxLength(320).IsRequired();
		builder.Property((User user) => user.PasswordHash).HasMaxLength(512).IsRequired();
		builder.Property((User user) => user.DisplayName).HasMaxLength(160);
		builder.Property((User user) => user.Role).HasMaxLength(64).IsRequired();
		builder.Property((User user) => user.SubscriptionTier).HasConversion<int>();
		builder.HasIndex((User user) => user.Email).IsUnique();
		builder.HasMany((User user) => user.Videos).WithOne((Video video) => video.User).HasForeignKey((Video video) => video.UserId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.HasMany((User user) => user.SocialAccounts).WithOne((SocialAccount account) => account.User).HasForeignKey((SocialAccount account) => account.UserId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.HasMany((User user) => user.RefreshTokens).WithOne((RefreshToken token) => token.User).HasForeignKey((RefreshToken token) => token.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
