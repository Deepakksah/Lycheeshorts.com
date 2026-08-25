using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lychee.Publisher.Application.Abstractions;

public interface IVideoService
{
	Task<VideoResponse> SubmitVideoAsync(Guid userId, SubmitVideoRequest request, CancellationToken cancellationToken);

	Task<VideoResponse> SubmitYouTubeImportAsync(Guid userId, YouTubeImportRequest request, CancellationToken cancellationToken);

	Task<IReadOnlyCollection<VideoResponse>> GetUserVideosAsync(Guid userId, CancellationToken cancellationToken);

	Task<VideoResponse?> GetVideoByIdAsync(Guid videoId, Guid userId, CancellationToken cancellationToken);

	Task<IReadOnlyCollection<ShortClipResponse>> GetVideoShortsAsync(Guid videoId, Guid userId, CancellationToken cancellationToken);

	Task<bool> TriggerVideoProcessingAsync(Guid videoId, Guid userId, TriggerProcessingRequest request, CancellationToken cancellationToken);

	Task<bool> StopVideoProcessingAsync(Guid videoId, Guid userId, CancellationToken cancellationToken);

	Task<bool> DeleteVideoAsync(Guid videoId, Guid userId, CancellationToken cancellationToken);

	Task<bool> DeleteShortClipAsync(Guid clipId, Guid userId, CancellationToken cancellationToken);
}

