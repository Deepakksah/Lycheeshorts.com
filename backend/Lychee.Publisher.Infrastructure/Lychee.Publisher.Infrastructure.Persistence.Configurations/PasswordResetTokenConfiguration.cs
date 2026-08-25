using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
	public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
	{
		builder.ToTable("PasswordResetTokens");
		builder.HasKey((PasswordResetToken token) => token.Id);
		builder.Property((PasswordResetToken token) => token.TokenHash).HasMaxLength(128).IsRequired();
		builder.Property((PasswordResetToken token) => token.CreatedByIp).HasMaxLength(64);
		builder.HasIndex((PasswordResetToken token) => token.TokenHash).IsUnique();
		builder.HasIndex((PasswordResetToken token) => new { token.UserId, token.ExpiresAtUtc });
		builder.HasOne((PasswordResetToken token) => token.User).WithMany((User user) => user.PasswordResetTokens).HasForeignKey((PasswordResetToken token) => token.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
