using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class VideoConfiguration : IEntityTypeConfiguration<Video>
{
	public void Configure(EntityTypeBuilder<Video> builder)
	{
		builder.ToTable("Videos");
		builder.HasKey((Video video) => video.Id);
		builder.Property((Video video) => video.SourceType).HasMaxLength(32).IsRequired();
		builder.Property((Video video) => video.SourceUrl).HasMaxLength(2048);
		builder.Property((Video video) => video.OriginalFileUri).HasMaxLength(2048).IsRequired();
		builder.Property((Video video) => video.ThumbnailUri).HasMaxLength(2048);
		builder.Property((Video video) => video.Title).HasMaxLength(280);
		builder.Property((Video video) => video.Status).HasConversion<int>();
		builder.HasIndex((Video video) => new { video.UserId, video.Status });
	}
}
