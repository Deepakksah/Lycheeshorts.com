namespace Lychee.Publisher.Api.Endpoints;

public record ConnectSocialAccountRequest(int Platform, string DisplayName, string? ChannelName, string? AuthCode, string? AccessToken, string? PasswordOrApiKey);
