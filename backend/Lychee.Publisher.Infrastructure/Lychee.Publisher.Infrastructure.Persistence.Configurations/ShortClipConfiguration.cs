using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class ShortClipConfiguration : IEntityTypeConfiguration<ShortClip>
{
	public void Configure(EntityTypeBuilder<ShortClip> builder)
	{
		builder.ToTable("Shorts");
		builder.HasKey((ShortClip shortClip) => shortClip.Id);
		builder.Property((ShortClip shortClip) => shortClip.OutputUri).HasMaxLength(2048).IsRequired();
		builder.Property((ShortClip shortClip) => shortClip.Title).HasMaxLength(280);
		builder.Property((ShortClip shortClip) => shortClip.Hashtags).HasMaxLength(1024);
		builder.Property((ShortClip shortClip) => shortClip.ViralityScore).HasPrecision(5, 4);
		builder.Property((ShortClip shortClip) => shortClip.Status).HasConversion<int>();
		builder.HasIndex((ShortClip shortClip) => new { shortClip.VideoId, shortClip.Status });
		builder.HasOne((ShortClip shortClip) => shortClip.Video).WithMany((Video video) => video.Shorts).HasForeignKey((ShortClip shortClip) => shortClip.VideoId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
