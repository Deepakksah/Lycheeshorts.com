namespace Lychee.Publisher.Application.Abstractions;

public sealed record UserUsageResponse(string CurrentPlanName, int CurrentTier, int VideosUsedThisMonth, int MonthlyVideoLimit, bool LimitExceeded);
