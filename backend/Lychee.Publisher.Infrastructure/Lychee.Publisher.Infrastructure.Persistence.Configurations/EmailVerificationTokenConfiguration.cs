using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
	public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
	{
		builder.ToTable("EmailVerificationTokens");
		builder.HasKey((EmailVerificationToken token) => token.Id);
		builder.Property((EmailVerificationToken token) => token.TokenHash).HasMaxLength(128).IsRequired();
		builder.Property((EmailVerificationToken token) => token.CreatedByIp).HasMaxLength(64);
		builder.HasIndex((EmailVerificationToken token) => token.TokenHash).IsUnique();
		builder.HasIndex((EmailVerificationToken token) => new { token.UserId, token.ExpiresAtUtc });
		builder.HasOne((EmailVerificationToken token) => token.User).WithMany((User user) => user.EmailVerificationTokens).HasForeignKey((EmailVerificationToken token) => token.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
