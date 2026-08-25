using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class PublishingScheduleConfiguration : IEntityTypeConfiguration<PublishingSchedule>
{
	public void Configure(EntityTypeBuilder<PublishingSchedule> builder)
	{
		builder.ToTable("Schedules");
		builder.HasKey((PublishingSchedule schedule) => schedule.Id);
		builder.Property((PublishingSchedule schedule) => schedule.Platform).HasConversion<int>();
		builder.Property((PublishingSchedule schedule) => schedule.Status).HasConversion<int>();
		builder.Property((PublishingSchedule schedule) => schedule.ExternalPostId).HasMaxLength(256);
		builder.Property((PublishingSchedule schedule) => schedule.FailureReason).HasMaxLength(1024);
		builder.HasIndex((PublishingSchedule schedule) => new { schedule.PublishAtUtc, schedule.Status });
		builder.HasOne((PublishingSchedule schedule) => schedule.ShortClip).WithMany((ShortClip shortClip) => shortClip.Schedules).HasForeignKey((PublishingSchedule schedule) => schedule.ShortClipId)
			.OnDelete(DeleteBehavior.Cascade);
		builder.HasOne((PublishingSchedule schedule) => schedule.SocialAccount).WithMany((SocialAccount account) => account.Schedules).HasForeignKey((PublishingSchedule schedule) => schedule.SocialAccountId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
