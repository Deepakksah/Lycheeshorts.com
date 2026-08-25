using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lychee.Publisher.Application.Abstractions;

public interface ISchedulingService
{
	Task<ScheduleResponse> CreateScheduleAsync(Guid userId, CreateScheduleRequest request, CancellationToken cancellationToken);

	Task<IReadOnlyCollection<ScheduleResponse>> GetUserSchedulesAsync(Guid userId, CancellationToken cancellationToken);

	Task<ScheduleResponse?> UpdateScheduleAsync(Guid scheduleId, Guid userId, UpdateScheduleRequest request, CancellationToken cancellationToken);

	Task<bool> DeleteScheduleAsync(Guid scheduleId, Guid userId, CancellationToken cancellationToken);
}
