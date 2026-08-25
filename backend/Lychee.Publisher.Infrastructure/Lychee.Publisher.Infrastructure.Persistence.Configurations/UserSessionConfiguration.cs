using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
	public void Configure(EntityTypeBuilder<UserSession> builder)
	{
		builder.ToTable("UserSessions");
		builder.HasKey((UserSession session) => session.Id);
		builder.Property((UserSession session) => session.IpAddress).HasMaxLength(64);
		builder.Property((UserSession session) => session.UserAgent).HasMaxLength(512);
		builder.HasIndex((UserSession session) => new { session.UserId, session.LastSeenAtUtc });
		builder.HasOne((UserSession session) => session.User).WithMany((User user) => user.Sessions).HasForeignKey((UserSession session) => session.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
