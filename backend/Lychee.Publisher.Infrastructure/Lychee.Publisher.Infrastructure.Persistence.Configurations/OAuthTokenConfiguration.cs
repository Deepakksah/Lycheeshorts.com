using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class OAuthTokenConfiguration : IEntityTypeConfiguration<OAuthToken>
{
	public void Configure(EntityTypeBuilder<OAuthToken> builder)
	{
		builder.ToTable("OAuthTokens");
		builder.HasKey((OAuthToken token) => token.Id);
		builder.Property((OAuthToken token) => token.AccessTokenCipherText).IsRequired();
		builder.Property((OAuthToken token) => token.RefreshTokenCipherText);
		builder.HasOne((OAuthToken token) => token.SocialAccount).WithOne().HasForeignKey((OAuthToken token) => token.SocialAccountId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
