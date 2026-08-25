using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class SocialAccountConfiguration : IEntityTypeConfiguration<SocialAccount>
{
	public void Configure(EntityTypeBuilder<SocialAccount> builder)
	{
		builder.ToTable("SocialAccounts");
		builder.HasKey((SocialAccount account) => account.Id);
		builder.Property((SocialAccount account) => account.Platform).HasConversion<int>();
		builder.Property((SocialAccount account) => account.ExternalAccountId).HasMaxLength(256).IsRequired();
		builder.Property((SocialAccount account) => account.DisplayName).HasMaxLength(160).IsRequired();
		builder.Property((SocialAccount account) => account.ChannelName).HasMaxLength(160);
		builder.HasIndex((SocialAccount account) => new { account.UserId, account.Platform, account.ExternalAccountId }).IsUnique();
	}
}
