using System;

namespace Lychee.Publisher.Api.Endpoints;

public record SocialAccountResponse(Guid Id, string Platform, string DisplayName, string? ChannelName, string ExternalAccountId, DateTimeOffset CreatedAtUtc);
