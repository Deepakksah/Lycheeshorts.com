using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lychee.Publisher.Infrastructure.Persistence;

public sealed class PublisherDbContext : DbContext
{
	public DbSet<User> Users => Set<User>();

	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

	public DbSet<Video> Videos => Set<Video>();

	public DbSet<ShortClip> Shorts => Set<ShortClip>();

	public DbSet<SocialAccount> SocialAccounts => Set<SocialAccount>();

	public DbSet<PublishingSchedule> Schedules => Set<PublishingSchedule>();

	public DbSet<Role> Roles => Set<Role>();

	public DbSet<Permission> Permissions => Set<Permission>();

	public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

	public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();

	public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

	public DbSet<UserSession> UserSessions => Set<UserSession>();

	public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

	public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();

	public DbSet<Payment> Payments => Set<Payment>();

	public DbSet<OAuthToken> OAuthTokens => Set<OAuthToken>();

	public PublisherDbContext(DbContextOptions<PublisherDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(PublisherDbContext).Assembly);
	}
}
