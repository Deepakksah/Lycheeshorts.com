using System;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class SocialPublisher : ISocialPublisher
{
	public PlatformType Platform => PlatformType.YouTube;

	public async Task<PublishResult> PublishAsync(SocialPublishRequest request, CancellationToken cancellationToken)
	{
		await Task.Delay(2000, cancellationToken);
		if (request.Title.Contains("fail", StringComparison.OrdinalIgnoreCase) || Random.Shared.Next(1, 100) <= 25)
		{
			string[] errors = new string[4] { "API Error: Platform upload quota limit exceeded for today. (Quota Code: 403)", "Network timeout during video chunk upload stream. SocketException: Connection reset by peer.", "OAuth token expired or revoked. Please re-authenticate your social channel.", "Platform Validation: Video aspect ratio must be strictly 9:16 vertical." };
			string randomError = errors[Random.Shared.Next(errors.Length)];
			return new PublishResult(Succeeded: false, null, randomError);
		}
		return new PublishResult(Succeeded: true, $"post_{Guid.NewGuid():N}", null);
	}
}
