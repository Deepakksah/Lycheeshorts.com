using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class AiContentService : IAiContentService
{
	public Task<GeneratedShortMetadata> GenerateShortMetadataAsync(string transcript, CancellationToken cancellationToken)
	{
		GeneratedShortMetadata result = new GeneratedShortMetadata("AI Generated Short", "Auto-generated description placeholder.", new global::_003C_003Ez__ReadOnlyArray<string>(new string[3] { "#shorts", "#ai", "#video" }), new global::_003C_003Ez__ReadOnlyArray<string>(new string[3] { "shorts", "reels", "ai video" }));
		return Task.FromResult(result);
	}
}
