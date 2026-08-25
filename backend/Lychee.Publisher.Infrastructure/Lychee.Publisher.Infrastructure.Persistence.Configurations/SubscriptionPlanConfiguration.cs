using System;
using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
	public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
	{
		builder.HasKey((SubscriptionPlan sp) => sp.Id);
		builder.Property((SubscriptionPlan sp) => sp.Name).HasMaxLength(100).IsRequired();
		builder.Property((SubscriptionPlan sp) => sp.MonthlyPrice).HasColumnType("decimal(18,2)").IsRequired();
		builder.Property((SubscriptionPlan sp) => sp.YearlyPrice).HasColumnType("decimal(18,2)").IsRequired();
		builder.Property((SubscriptionPlan sp) => sp.MonthlyVideoLimit).IsRequired();
		builder.Property((SubscriptionPlan sp) => sp.Tier).IsRequired();
		builder.HasData(new SubscriptionPlan
		{
			Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
			Tier = 1,
			Name = "Free",
			MonthlyPrice = 0.00m,
			YearlyPrice = 0.00m,
			MonthlyVideoLimit = 5,
			CreatedAtUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
		}, new SubscriptionPlan
		{
			Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
			Tier = 2,
			Name = "Pro",
			MonthlyPrice = 499.00m,
			YearlyPrice = 4990.00m,
			MonthlyVideoLimit = 50,
			CreatedAtUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
		}, new SubscriptionPlan
		{
			Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
			Tier = 3,
			Name = "Agency",
			MonthlyPrice = 1999.00m,
			YearlyPrice = 19990.00m,
			MonthlyVideoLimit = 200,
			CreatedAtUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
		}, new SubscriptionPlan
		{
			Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
			Tier = 4,
			Name = "Enterprise",
			MonthlyPrice = 9999.00m,
			YearlyPrice = 99990.00m,
			MonthlyVideoLimit = 5000,
			CreatedAtUtc = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
		});
	}
}
