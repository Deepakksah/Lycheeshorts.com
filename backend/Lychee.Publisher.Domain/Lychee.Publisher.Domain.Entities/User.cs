using System;
using System.Collections.Generic;
using Lychee.Publisher.Domain.Common;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Domain.Entities;

public sealed class User : AuditableEntity
{
	public required string Email { get; set; }

	public required string PasswordHash { get; set; }

	public string? DisplayName { get; set; }

	public bool IsEmailVerified { get; set; }

	public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;

	public string Role { get; set; } = "User";

	public DateTimeOffset? LastLoginAtUtc { get; set; }

	public ICollection<Video> Videos { get; set; } = new List<Video>();

	public ICollection<SocialAccount> SocialAccounts { get; set; } = new List<SocialAccount>();

	public ICollection<Payment> Payments { get; set; } = new List<Payment>();

	public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

	public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();

	public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

	public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
}
