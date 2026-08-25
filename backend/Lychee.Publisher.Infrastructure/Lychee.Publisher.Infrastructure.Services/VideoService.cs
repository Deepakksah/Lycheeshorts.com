using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Domain.Enums;
using Lychee.Publisher.Infrastructure.Jobs;
using Lychee.Publisher.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class VideoService(PublisherDbContext dbContext) : IVideoService
{
	private async Task EnforceVideoLimitAsync(Guid userId, CancellationToken cancellationToken)
	{
		User user = await dbContext.Users.FindAsync(new object[1] { userId }, cancellationToken);
		if (user == null)
		{
			throw new InvalidOperationException("User not found.");
		}
		int limit = (await dbContext.SubscriptionPlans.ToListAsync(cancellationToken)).FirstOrDefault((SubscriptionPlan p) => p.Tier == (int)user.SubscriptionTier)?.MonthlyVideoLimit ?? 5;
		DateTimeOffset firstDayOfMonth = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);
		List<Video> userVideos = await dbContext.Videos.Where((Video v) => v.UserId == userId).ToListAsync(cancellationToken);
		if (userVideos.Count((Video v) => v.CreatedAtUtc >= firstDayOfMonth) >= limit)
		{
			throw new InvalidOperationException($"Monthly video limit of {limit} reached. Please upgrade to import or upload more videos.");
		}
	}

	public async Task<VideoResponse> SubmitVideoAsync(Guid userId, SubmitVideoRequest request, CancellationToken cancellationToken)
	{
		await EnforceVideoLimitAsync(userId, cancellationToken);
		Video video = new Video
		{
			UserId = userId,
			SourceType = "Upload",
			OriginalFileUri = request.OriginalFileUri,
			Title = (request.Title ?? "Uploaded Video"),
			Duration = request.Duration,
			Status = ProcessingStatus.Uploaded
		};
		dbContext.Videos.Add(video);
		await dbContext.SaveChangesAsync(cancellationToken);
		return MapToResponse(video);
	}

	public async Task<VideoResponse> SubmitYouTubeImportAsync(Guid userId, YouTubeImportRequest request, CancellationToken cancellationToken)
	{
		await EnforceVideoLimitAsync(userId, cancellationToken);
		Video video = new Video
		{
			UserId = userId,
			SourceType = "YouTube",
			SourceUrl = request.GetEffectiveUrl(),
			OriginalFileUri = string.Empty,
			Title = (!string.IsNullOrWhiteSpace(request.Title) ? request.Title : "YouTube Imported Video"),
			Status = ProcessingStatus.Queued
		};
		dbContext.Videos.Add(video);
		await dbContext.SaveChangesAsync(cancellationToken);
		BackgroundJob.Enqueue((VideoProcessingJob job) => job.ProcessVideoAsync(video.Id, new TriggerProcessingRequest(), CancellationToken.None));
		return MapToResponse(video);
	}

	public async Task<IReadOnlyCollection<VideoResponse>> GetUserVideosAsync(Guid userId, CancellationToken cancellationToken)
	{
		List<Video> list = await dbContext.Videos.AsNoTracking().Where((Video v) => v.UserId == userId).ToListAsync(cancellationToken);
		return list.OrderByDescending((Video v) => v.CreatedAtUtc).Select(MapToResponse).ToList();
	}

	public async Task<VideoResponse?> GetVideoByIdAsync(Guid videoId, Guid userId, CancellationToken cancellationToken)
	{
		Video video = await dbContext.Videos.AsNoTracking().SingleOrDefaultAsync((Video v) => v.Id == videoId && v.UserId == userId, cancellationToken);
		return (video == null) ? null : MapToResponse(video);
	}

	public async Task<IReadOnlyCollection<ShortClipResponse>> GetVideoShortsAsync(Guid videoId, Guid userId, CancellationToken cancellationToken)
	{
		if (!(await dbContext.Videos.AnyAsync((Video v) => v.Id == videoId && v.UserId == userId, cancellationToken)))
		{
			return Array.Empty<ShortClipResponse>();
		}
		List<ShortClip> list = await dbContext.Shorts.AsNoTracking().Where((ShortClip s) => s.VideoId == videoId).ToListAsync(cancellationToken);
		return list.OrderByDescending((ShortClip s) => s.ViralityScore).Select(MapToShortResponse).ToList();
	}

	public async Task<bool> TriggerVideoProcessingAsync(Guid videoId, Guid userId, TriggerProcessingRequest request, CancellationToken cancellationToken)
	{
		Video video = await dbContext.Videos.SingleOrDefaultAsync((Video v) => v.Id == videoId && v.UserId == userId, cancellationToken);
		if (video == null)
		{
			return false;
		}
		video.Status = ProcessingStatus.Queued;
		await dbContext.SaveChangesAsync(cancellationToken);
		BackgroundJob.Enqueue((VideoProcessingJob job) => job.ProcessVideoAsync(video.Id, request, CancellationToken.None));
		return true;
	}

	public async Task<bool> StopVideoProcessingAsync(Guid videoId, Guid userId, CancellationToken cancellationToken)
	{
		Video video = await dbContext.Videos.SingleOrDefaultAsync((Video v) => v.Id == videoId && v.UserId == userId, cancellationToken);
		if (video == null) return false;

		video.Status = ProcessingStatus.Cancelled;
		await dbContext.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteVideoAsync(Guid videoId, Guid userId, CancellationToken cancellationToken)
	{
		Video video = await dbContext.Videos.SingleOrDefaultAsync((Video v) => v.Id == videoId && v.UserId == userId, cancellationToken);
		if (video == null) return false;

		List<ShortClip> shorts = await dbContext.Shorts.Where((ShortClip s) => s.VideoId == videoId).ToListAsync(cancellationToken);
		foreach (ShortClip shortClip in shorts)
		{
			List<PublishingSchedule> clipSchedules = await dbContext.Schedules.Where((PublishingSchedule sc) => sc.ShortClipId == shortClip.Id).ToListAsync(cancellationToken);
			dbContext.Schedules.RemoveRange(clipSchedules);
		}
		dbContext.Shorts.RemoveRange(shorts);
		dbContext.Videos.Remove(video);
		await dbContext.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> DeleteShortClipAsync(Guid clipId, Guid userId, CancellationToken cancellationToken)
	{
		ShortClip clip = await dbContext.Shorts.Include((ShortClip s) => s.Video).SingleOrDefaultAsync((ShortClip s) => s.Id == clipId && s.Video.UserId == userId, cancellationToken);
		if (clip == null) return false;

		List<PublishingSchedule> schedules = await dbContext.Schedules.Where((PublishingSchedule sc) => sc.ShortClipId == clipId).ToListAsync(cancellationToken);
		dbContext.Schedules.RemoveRange(schedules);
		dbContext.Shorts.Remove(clip);
		await dbContext.SaveChangesAsync(cancellationToken);
		return true;
	}


	private static VideoResponse MapToResponse(Video video)
	{
		return new VideoResponse(video.Id, video.UserId, video.SourceType, video.SourceUrl, video.OriginalFileUri, video.ThumbnailUri, video.Title, video.Duration, video.Status.ToString(), video.CreatedAtUtc);
	}

	private static ShortClipResponse MapToShortResponse(ShortClip clip)
	{
		return new ShortClipResponse(clip.Id, clip.VideoId, clip.OutputUri, clip.Title, clip.Description, clip.Hashtags, clip.CaptionsJson, clip.ViralityScore, clip.StartTime, clip.EndTime, clip.Status.ToString(), clip.CreatedAtUtc);
	}
}
