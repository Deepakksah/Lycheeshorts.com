using System;
using System.Threading.Tasks;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Lychee.Publisher.Infrastructure.Persistence;

public static class DbSeeder
{
	private static readonly Guid RoleAdminId = new Guid("10000000-0000-0000-0000-000000000001");

	private static readonly Guid RoleUserId = new Guid("10000000-0000-0000-0000-000000000002");

	private static readonly Guid RoleAgencyId = new Guid("10000000-0000-0000-0000-000000000003");

	private static readonly Guid PlanFreeId = new Guid("20000000-0000-0000-0000-000000000001");

	private static readonly Guid PlanStarterId = new Guid("20000000-0000-0000-0000-000000000002");

	private static readonly Guid PlanProId = new Guid("20000000-0000-0000-0000-000000000003");

	private static readonly Guid PlanAgencyId = new Guid("20000000-0000-0000-0000-000000000004");

	private static readonly Guid PlanEnterpriseId = new Guid("20000000-0000-0000-0000-000000000005");

	public static async Task SeedAsync(PublisherDbContext db, IPasswordHasher<User>? passwordHasher = null)
	{
		await SeedRolesAsync(db);
		await SeedPlansAsync(db);
		await SeedAdminUserAsync(db, passwordHasher);
		await db.SaveChangesAsync();
	}

	private static async Task SeedRolesAsync(PublisherDbContext db)
	{
		string[] seedRoles = new string[3] { "Admin", "User", "Agency" };
		string[] array = seedRoles;
		foreach (string roleName in array)
		{
			if (!(await db.Roles.AnyAsync((Role r) => r.Name == roleName)))
			{
				db.Roles.Add(new Role
				{
					Id = Guid.NewGuid(),
					Name = roleName
				});
			}
		}
	}

	private static async Task SeedPlansAsync(PublisherDbContext db)
	{
		SubscriptionPlan[] seedPlans = new SubscriptionPlan[5]
		{
			new SubscriptionPlan
			{
				Id = PlanFreeId,
				Tier = 1,
				Name = "Free",
				MonthlyPrice = 0m,
				YearlyPrice = 0m,
				MonthlyVideoLimit = 5
			},
			new SubscriptionPlan
			{
				Id = PlanStarterId,
				Tier = 2,
				Name = "Starter",
				MonthlyPrice = 499m,
				YearlyPrice = 4490m,
				MonthlyVideoLimit = 20
			},
			new SubscriptionPlan
			{
				Id = PlanProId,
				Tier = 3,
				Name = "Pro",
				MonthlyPrice = 999m,
				YearlyPrice = 8990m,
				MonthlyVideoLimit = 100
			},
			new SubscriptionPlan
			{
				Id = PlanAgencyId,
				Tier = 4,
				Name = "Agency",
				MonthlyPrice = 2499m,
				YearlyPrice = 22490m,
				MonthlyVideoLimit = 500
			},
			new SubscriptionPlan
			{
				Id = PlanEnterpriseId,
				Tier = 5,
				Name = "Enterprise",
				MonthlyPrice = 4999m,
				YearlyPrice = 44990m,
				MonthlyVideoLimit = int.MaxValue
			}
		};
		SubscriptionPlan[] array = seedPlans;
		foreach (SubscriptionPlan plan in array)
		{
			if (!(await db.SubscriptionPlans.AnyAsync((SubscriptionPlan p) => p.Tier == plan.Tier)))
			{
				db.SubscriptionPlans.Add(plan);
			}
		}
	}

	private static async Task SeedAdminUserAsync(PublisherDbContext db, IPasswordHasher<User>? passwordHasher)
	{
		User existingAdmin = await db.Users.FirstOrDefaultAsync((User u) => u.Email.ToLower() == "admin@lychee.com");
		IPasswordHasher<User> hasher = passwordHasher ?? new PasswordHasher<User>(Options.Create(new PasswordHasherOptions()));
		if (existingAdmin == null)
		{
			User adminUser = new User
			{
				Id = Guid.NewGuid(),
				Email = "admin@lychee.com",
				PasswordHash = string.Empty,
				DisplayName = "Admin",
				Role = "Admin",
				SubscriptionTier = SubscriptionTier.Enterprise,
				IsEmailVerified = true,
				CreatedAtUtc = DateTimeOffset.UtcNow
			};
			adminUser.PasswordHash = hasher.HashPassword(adminUser, "123456");
			db.Users.Add(adminUser);
		}
		else
		{
			existingAdmin.Role = "Admin";
			existingAdmin.SubscriptionTier = SubscriptionTier.Enterprise;
			existingAdmin.IsEmailVerified = true;
			existingAdmin.PasswordHash = hasher.HashPassword(existingAdmin, "123456");
		}
	}
}
