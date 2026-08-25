using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("RefreshTokens");
		builder.HasKey((RefreshToken token) => token.Id);
		builder.Property((RefreshToken token) => token.TokenHash).HasMaxLength(128).IsRequired();
		builder.Property((RefreshToken token) => token.ReplacedByTokenHash).HasMaxLength(128);
		builder.Property((RefreshToken token) => token.CreatedByIp).HasMaxLength(64);
		builder.Property((RefreshToken token) => token.RevokedByIp).HasMaxLength(64);
		builder.HasIndex((RefreshToken token) => token.TokenHash).IsUnique();
		builder.HasIndex((RefreshToken token) => new { token.UserId, token.ExpiresAtUtc });
	}
}
