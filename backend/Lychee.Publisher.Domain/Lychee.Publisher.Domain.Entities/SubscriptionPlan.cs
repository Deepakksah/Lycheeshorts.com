using System;

namespace Lychee.Publisher.Domain.Entities;

public sealed class SubscriptionPlan
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public int Tier { get; set; }

	public required string Name { get; set; }

	public decimal MonthlyPrice { get; set; }

	public decimal YearlyPrice { get; set; }

	public int MonthlyVideoLimit { get; set; }

	public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
