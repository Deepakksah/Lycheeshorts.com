using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Domain.Enums;
using Lychee.Publisher.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lychee.Publisher.Infrastructure.Jobs;

public sealed class PublishScheduledClipsJob(PublisherDbContext dbContext, ISocialPublisher socialPublisher)
{
	[Queue("default")]
	[AutomaticRetry(Attempts = 3)]
	public async Task PublishPendingClipsAsync(CancellationToken cancellationToken)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;
		List<PublishingSchedule> pendingSchedules = await (from s in dbContext.Schedules.Include((PublishingSchedule s) => s.ShortClip).Include((PublishingSchedule s) => s.SocialAccount)
			where (int)s.Status == 6 && s.PublishAtUtc <= now
			select s).ToListAsync(cancellationToken);
		if (pendingSchedules.Count == 0)
		{
			return;
		}
		foreach (PublishingSchedule schedule in pendingSchedules)
		{
			schedule.Status = ProcessingStatus.Publishing;
		}
		await dbContext.SaveChangesAsync(cancellationToken);
		foreach (PublishingSchedule schedule2 in pendingSchedules)
		{
			if (schedule2.ShortClip == null || schedule2.SocialAccount == null)
			{
				schedule2.Status = ProcessingStatus.Failed;
				schedule2.FailureReason = "Associated short clip or social account was deleted.";
				continue;
			}
			try
			{
				SocialPublishRequest publishRequest = new SocialPublishRequest(Hashtags: (from h in (schedule2.ShortClip.Hashtags ?? string.Empty).Split(new char[3] { ' ', ',', '#' }, StringSplitOptions.RemoveEmptyEntries)
					select h.Trim()).ToList(), ShortClipId: schedule2.ShortClip.Id, SocialAccountId: schedule2.SocialAccount.Id, Platform: schedule2.SocialAccount.Platform, VideoUri: schedule2.ShortClip.OutputUri, Title: schedule2.ShortClip.Title ?? "New Short", Description: schedule2.ShortClip.Description ?? string.Empty);
				PublishResult result = await socialPublisher.PublishAsync(publishRequest, cancellationToken);
				if (result.Succeeded)
				{
					schedule2.Status = ProcessingStatus.Published;
					schedule2.ExternalPostId = result.ExternalPostId;
				}
				else
				{
					schedule2.Status = ProcessingStatus.Failed;
					schedule2.FailureReason = result.ErrorMessage ?? "Unknown publishing error.";
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				schedule2.Status = ProcessingStatus.Failed;
				schedule2.FailureReason = ex2.Message;
			}
		}
		await dbContext.SaveChangesAsync(cancellationToken);
	}
}
