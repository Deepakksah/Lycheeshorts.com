using System;
using Lychee.Publisher.Domain.Common;

namespace Lychee.Publisher.Domain.Entities;

public sealed class OAuthToken : AuditableEntity
{
	public Guid SocialAccountId { get; set; }

	public SocialAccount? SocialAccount { get; set; }

	public required string AccessTokenCipherText { get; set; }

	public string? RefreshTokenCipherText { get; set; }

	public DateTimeOffset? ExpiresAtUtc { get; set; }
}
