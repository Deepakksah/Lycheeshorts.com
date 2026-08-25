using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Domain.Enums;
using Lychee.Publisher.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class SchedulingService(PublisherDbContext dbContext) : ISchedulingService
{
	public async Task<ScheduleResponse> CreateScheduleAsync(Guid userId, CreateScheduleRequest request, CancellationToken cancellationToken)
	{
		ShortClip shortClip = await dbContext.Shorts.Include((ShortClip s) => s.Video).SingleOrDefaultAsync((ShortClip s) => s.Id == request.ShortClipId, cancellationToken);
		int num;
		if (shortClip != null)
		{
			Video? video = shortClip.Video;
			num = ((video == null || video.UserId != userId) ? 1 : 0);
		}
		else
		{
			num = 1;
		}
		if (num != 0)
		{
			throw new InvalidOperationException("Invalid short clip selected or access denied.");
		}
		if (await dbContext.SocialAccounts.SingleOrDefaultAsync((SocialAccount sa) => sa.Id == request.SocialAccountId && sa.UserId == userId, cancellationToken) == null && await dbContext.SocialAccounts.FirstOrDefaultAsync((SocialAccount sa) => sa.UserId == userId && (int)sa.Platform == (int)request.Platform, cancellationToken) == null)
		{
			SocialAccount socialAccount = new SocialAccount
			{
				Id = ((request.SocialAccountId == Guid.Empty) ? Guid.NewGuid() : request.SocialAccountId),
				UserId = userId,
				Platform = request.Platform,
				ExternalAccountId = "mock_" + request.Platform.ToString().ToLower() + "_" + Guid.NewGuid().ToString().Substring(0, 8),
				DisplayName = $"My Mock {request.Platform} Channel",
				ChannelName = $"Mock {request.Platform}",
				IsActive = true
			};
			dbContext.SocialAccounts.Add(socialAccount);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
		PublishingSchedule schedule = new PublishingSchedule
		{
			ShortClipId = request.ShortClipId,
			SocialAccountId = request.SocialAccountId,
			Platform = request.Platform,
			PublishAtUtc = request.PublishAtUtc,
			Status = ProcessingStatus.Scheduled
		};
		dbContext.Schedules.Add(schedule);
		await dbContext.SaveChangesAsync(cancellationToken);
		return MapToResponse(schedule);
	}

	public async Task<IReadOnlyCollection<ScheduleResponse>> GetUserSchedulesAsync(Guid userId, CancellationToken cancellationToken)
	{
		List<PublishingSchedule> list = await dbContext.Schedules.AsNoTracking().Include((PublishingSchedule s) => s.SocialAccount).Where((PublishingSchedule s) => s.SocialAccount != null && s.SocialAccount.UserId == userId).ToListAsync(cancellationToken);
		return list.OrderByDescending((PublishingSchedule s) => s.PublishAtUtc).Select(MapToResponse).ToList();
	}

	public async Task<ScheduleResponse?> UpdateScheduleAsync(Guid scheduleId, Guid userId, UpdateScheduleRequest request, CancellationToken cancellationToken)
	{
		PublishingSchedule schedule = await dbContext.Schedules.Include((PublishingSchedule s) => s.SocialAccount).SingleOrDefaultAsync((PublishingSchedule s) => s.Id == scheduleId && s.SocialAccount != null && s.SocialAccount.UserId == userId, cancellationToken);
		if (schedule == null)
		{
			return null;
		}
		if (schedule.Status == ProcessingStatus.Published || schedule.Status == ProcessingStatus.Publishing)
		{
			throw new InvalidOperationException("Cannot modify a schedule that is already published or in progress.");
		}
		schedule.PublishAtUtc = request.PublishAtUtc;
		schedule.Status = ProcessingStatus.Scheduled;
		schedule.FailureReason = null;
		await dbContext.SaveChangesAsync(cancellationToken);
		return MapToResponse(schedule);
	}

	public async Task<bool> DeleteScheduleAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken)
	{
		PublishingSchedule schedule = await dbContext.Schedules.Include((PublishingSchedule s) => s.SocialAccount).SingleOrDefaultAsync((PublishingSchedule s) => s.Id == scheduleId && s.SocialAccount != null && s.SocialAccount.UserId == userId, cancellationToken);
		if (schedule == null)
		{
			return false;
		}
		if (schedule.Status == ProcessingStatus.Published || schedule.Status == ProcessingStatus.Publishing)
		{
			throw new InvalidOperationException("Cannot delete a schedule that is already published or in progress.");
		}
		dbContext.Schedules.Remove(schedule);
		await dbContext.SaveChangesAsync(cancellationToken);
		return true;
	}

	private static ScheduleResponse MapToResponse(PublishingSchedule schedule)
	{
		return new ScheduleResponse(schedule.Id, schedule.ShortClipId, schedule.SocialAccountId, schedule.Platform.ToString(), schedule.PublishAtUtc, schedule.Status.ToString(), schedule.ExternalPostId, schedule.FailureReason, schedule.CreatedAtUtc);
	}
}
