using System;

namespace Lychee.Publisher.Api.Endpoints;

public record AdminUserDto(Guid Id, string Email, string? DisplayName, string Role, string SubscriptionTier, bool IsEmailVerified, DateTimeOffset? LastLoginAtUtc, DateTimeOffset CreatedAtUtc);
