using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record SubscriptionPlanResponse(Guid Id, int Tier, string Name, decimal MonthlyPrice, decimal YearlyPrice, int MonthlyVideoLimit);
