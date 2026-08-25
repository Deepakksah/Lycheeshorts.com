using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Application.Abstractions;

public interface ISocialPublisher
{
	PlatformType Platform { get; }

	Task<PublishResult> PublishAsync(SocialPublishRequest request, CancellationToken cancellationToken);
}
