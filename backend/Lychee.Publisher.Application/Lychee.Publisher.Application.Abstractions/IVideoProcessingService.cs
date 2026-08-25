using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lychee.Publisher.Application.Abstractions;

public interface IVideoProcessingService
{
	Task<IReadOnlyCollection<GeneratedShortClip>> GenerateShortsAsync(VideoProcessingRequest request, CancellationToken cancellationToken);
}
