using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Domain.Enums;
using Lychee.Publisher.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Lychee.Publisher.Api.Endpoints;

public static class SocialAccountEndpoints
{
	public static IEndpointRouteBuilder MapSocialAccountEndpoints(this IEndpointRouteBuilder app)
	{
		RouteGroupBuilder endpoints = app.MapGroup("/api/v1/social-accounts").RequireAuthorization().WithTags("SocialAccounts");
		endpoints.MapGet("/", (Func<ClaimsPrincipal, PublisherDbContext, CancellationToken, Task<IResult>>)async delegate(ClaimsPrincipal user, PublisherDbContext dbContext, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			return (userId == Guid.Empty) ? Results.Unauthorized() : Results.Ok(await (from sa in dbContext.SocialAccounts.AsNoTracking()
				where sa.UserId == userId && sa.IsActive
				select new SocialAccountResponse(sa.Id, sa.Platform.ToString(), sa.DisplayName, sa.ChannelName, sa.ExternalAccountId, sa.CreatedAtUtc)).ToListAsync(cancellationToken));
		}).WithName("GetSocialAccounts");
		endpoints.MapPost("/connect", (Func<ConnectSocialAccountRequest, ClaimsPrincipal, PublisherDbContext, IDataEncryptionService, CancellationToken, Task<IResult>>)async delegate(ConnectSocialAccountRequest request, ClaimsPrincipal user, PublisherDbContext dbContext, IDataEncryptionService encryptionService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			if (userId == Guid.Empty)
			{
				return Results.Unauthorized();
			}
			if (!Enum.IsDefined(typeof(PlatformType), request.Platform))
			{
				return Results.BadRequest("Invalid platform type.");
			}
			PlatformType platform = (PlatformType)request.Platform;
			if (await dbContext.SocialAccounts.FirstOrDefaultAsync((SocialAccount sa) => sa.UserId == userId && sa.Platform == platform && sa.DisplayName == request.DisplayName, cancellationToken) != null)
			{
				return Results.BadRequest("Channel '" + request.DisplayName + "' is already connected on this platform.");
			}
			bool hasAuthCode = !string.IsNullOrWhiteSpace(request.AuthCode);
			bool hasAccessToken = !string.IsNullOrWhiteSpace(request.AccessToken);
			bool hasSecretKey = !string.IsNullOrWhiteSpace(request.PasswordOrApiKey);
			if (!hasAuthCode && !hasAccessToken && !hasSecretKey)
			{
				return Results.BadRequest(new
				{
					message = "Authentication Failed: Valid OAuth Authorization Code, Access Token, or Password/API Key is required to verify channel ownership."
				});
			}
			SocialAccount socialAccount = new SocialAccount
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				Platform = platform,
				ExternalAccountId = $"ext_{platform.ToString().ToLower()}_{Guid.NewGuid():N}",
				DisplayName = request.DisplayName,
				ChannelName = (request.ChannelName ?? request.DisplayName),
				IsActive = true
			};
			dbContext.SocialAccounts.Add(socialAccount);
			string activeAccessToken = (hasAccessToken ? request.AccessToken : (hasAuthCode ? ("auth_code_token_" + request.AuthCode) : ("api_key_token_" + request.PasswordOrApiKey)));
			string activeRefreshToken = $"refresh_token_for_{platform.ToString().ToLower()}_{Guid.NewGuid():N}";
			OAuthToken oauthToken = new OAuthToken
			{
				Id = Guid.NewGuid(),
				SocialAccountId = socialAccount.Id,
				AccessTokenCipherText = encryptionService.Encrypt(activeAccessToken),
				RefreshTokenCipherText = encryptionService.Encrypt(activeRefreshToken),
				ExpiresAtUtc = DateTimeOffset.UtcNow.AddDays(30.0)
			};
			dbContext.OAuthTokens.Add(oauthToken);
			dbContext.AuditLogs.Add(new AuditLog
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				Action = "CONNECT_SOCIAL_ACCOUNT",
				EntityName = "SocialAccount",
				EntityId = socialAccount.Id.ToString(),
				MetadataJson = $"{{\"Details\":\"Successfully connected {platform} account: {request.DisplayName}\",\"IpAddress\":\"127.0.0.1\",\"UserAgent\":\"Web-App-Client\"}}"
			});
			await dbContext.SaveChangesAsync(cancellationToken);
			return Results.Created($"/api/v1/social-accounts/{socialAccount.Id}", new SocialAccountResponse(socialAccount.Id, socialAccount.Platform.ToString(), socialAccount.DisplayName, socialAccount.ChannelName, socialAccount.ExternalAccountId, socialAccount.CreatedAtUtc));
		}).WithName("ConnectSocialAccount");
		endpoints.MapGet("/oauth-url", (Func<int, IConfiguration, IResult>)delegate(int platform, IConfiguration config)
		{
			if (platform == 1)
			{
				string text = config["SocialPlatforms:YouTube:ClientId"];
				string stringToEscape = config["SocialPlatforms:YouTube:RedirectUri"] ?? "http://localhost:3000/oauth/callback?platform=1";
				bool flag = !string.IsNullOrEmpty(text) && !text.StartsWith("YOUR_");
				string value = Uri.EscapeDataString("https://www.googleapis.com/auth/youtube.upload https://www.googleapis.com/auth/youtube.readonly");
				string url = (flag ? $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={text}&redirect_uri={Uri.EscapeDataString(stringToEscape)}&scope={value}&access_type=offline&prompt=consent" : "http://localhost:3000/oauth/callback?platform=1");
				return Results.Ok(new
				{
					url = url,
					configured = flag
				});
			}
			string text2 = config["SocialPlatforms:Meta:AppId"];
			string stringToEscape2 = config["SocialPlatforms:Meta:RedirectUri"] ?? $"http://localhost:3000/oauth/callback?platform={platform}";
			bool flag2 = !string.IsNullOrEmpty(text2) && !text2.StartsWith("YOUR_");
			string value2 = Uri.EscapeDataString("instagram_basic,instagram_content_publish,pages_show_list,pages_read_engagement,publish_video");
			string url2 = (flag2 ? $"https://www.facebook.com/v20.0/dialog/oauth?client_id={text2}&redirect_uri={Uri.EscapeDataString(stringToEscape2)}&scope={value2}" : $"http://localhost:3000/oauth/callback?platform={platform}");
			return Results.Ok(new
			{
				url = url2,
				configured = flag2
			});
		}).WithName("GetOAuthUrl");
		endpoints.MapDelete("/{id:guid}", (Func<Guid, ClaimsPrincipal, PublisherDbContext, CancellationToken, Task<IResult>>)async delegate(Guid id, ClaimsPrincipal user, PublisherDbContext dbContext, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			if (userId == Guid.Empty)
			{
				return Results.Unauthorized();
			}
			SocialAccount socialAccount = await dbContext.SocialAccounts.SingleOrDefaultAsync((SocialAccount sa) => sa.Id == id && sa.UserId == userId, cancellationToken);
			if (socialAccount == null)
			{
				return Results.NotFound();
			}
			dbContext.SocialAccounts.Remove(socialAccount);
			dbContext.AuditLogs.Add(new AuditLog
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				Action = "DISCONNECT_SOCIAL_ACCOUNT",
				EntityName = "SocialAccount",
				EntityId = id.ToString(),
				MetadataJson = $"{{\"Details\":\"Successfully disconnected {socialAccount.Platform} account: {socialAccount.DisplayName}\",\"IpAddress\":\"127.0.0.1\",\"UserAgent\":\"Web-App-Client\"}}"
			});
			await dbContext.SaveChangesAsync(cancellationToken);
			return Results.NoContent();
		}).WithName("DisconnectSocialAccount");
		return app;
	}

	private static Guid GetUserId(ClaimsPrincipal user)
	{
		string input = user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
		Guid result;
		return Guid.TryParse(input, out result) ? result : Guid.Empty;
	}
}
