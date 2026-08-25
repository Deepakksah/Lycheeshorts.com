using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Domain.Enums;
using Lychee.Publisher.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Lychee.Publisher.Api.Endpoints;

public static class AdminEndpoints
{
	public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
	{
		RouteGroupBuilder endpoints = app.MapGroup("/api/v1/admin").RequireAuthorization().WithTags("Admin");
		endpoints.MapGet("/users", (Func<int, int, string, PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(int page, int pageSize, string? search, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			page = Math.Max(1, page);
			pageSize = Math.Clamp(pageSize, 5, 100);
			IQueryable<User> query = db.Users.AsNoTracking();
			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where((User u) => u.Email.Contains(search) || (u.DisplayName != null && u.DisplayName.Contains(search)));
			}
			return Results.Ok(new
			{
				total = await query.CountAsync(ct),
				page = page,
				pageSize = pageSize,
				users = await (from u in query.OrderByDescending((User u) => u.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize)
					select new AdminUserDto(u.Id, u.Email, u.DisplayName, u.Role, u.SubscriptionTier.ToString(), u.IsEmailVerified, u.LastLoginAtUtc, u.CreatedAtUtc)).ToListAsync(ct)
			});
		}).WithName("AdminGetUsers");
		endpoints.MapPatch("/users/{id:guid}/role", (Func<Guid, ChangeRoleRequest, PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(Guid id, ChangeRoleRequest request, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			User user = await db.Users.FindAsync(new object[1] { id }, ct);
			if (user == null)
			{
				return Results.NotFound();
			}
			user.Role = request.Role;
			user.UpdatedAtUtc = DateTimeOffset.UtcNow;
			await db.SaveChangesAsync(ct);
			return Results.Ok(new
			{
				userId = id,
				newRole = request.Role
			});
		}).WithName("AdminChangeUserRole");
		endpoints.MapPatch("/users/{id:guid}/tier", (Func<Guid, ChangeSubscriptionTierRequest, PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(Guid id, ChangeSubscriptionTierRequest request, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			User user = await db.Users.FindAsync(new object[1] { id }, ct);
			if (user == null)
			{
				return Results.NotFound();
			}
			if (!Enum.TryParse<SubscriptionTier>(request.Tier, ignoreCase: true, out var tier))
			{
				return Results.BadRequest("Invalid subscription tier.");
			}
			user.SubscriptionTier = tier;
			user.UpdatedAtUtc = DateTimeOffset.UtcNow;
			await db.SaveChangesAsync(ct);
			return Results.Ok(new
			{
				userId = id,
				newTier = tier.ToString()
			});
		}).WithName("AdminChangeUserTier");
		endpoints.MapDelete("/users/{id:guid}", (Func<Guid, PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(Guid id, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			User user = await db.Users.FindAsync(new object[1] { id }, ct);
			if (user == null)
			{
				return Results.NotFound();
			}
			db.Users.Remove(user);
			await db.SaveChangesAsync(ct);
			return Results.NoContent();
		}).WithName("AdminDeleteUser");
		endpoints.MapGet("/subscriptions", (Func<int, int, PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(int page, int pageSize, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			page = Math.Max(1, page);
			pageSize = Math.Clamp(pageSize, 5, 100);
			return Results.Ok(new
			{
				total = await db.Payments.CountAsync(ct),
				page = page,
				pageSize = pageSize,
				rows = await (from p in (from p in db.Payments.AsNoTracking().Include((Payment p) => p.User)
						orderby p.CreatedAtUtc descending
						select p).Skip((page - 1) * pageSize).Take(pageSize)
					select new AdminSubscriptionDto(p.Id, p.User.Email, p.User.DisplayName, p.Provider, p.ProviderPaymentId, p.Amount, p.Currency, p.Status, p.CreatedAtUtc)).ToListAsync(ct)
			});
		}).WithName("AdminGetSubscriptions");
		endpoints.MapGet("/revenue", (Func<PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			List<Payment> allPayments = await db.Payments.AsNoTracking().ToListAsync(ct);
			decimal totalRevenue = allPayments.Where((Payment p) => p.Status == "Completed").Sum((Payment p) => p.Amount);
			decimal monthlyRevenue = allPayments.Where((Payment p) => p.Status == "Completed" && p.CreatedAtUtc >= DateTimeOffset.UtcNow.AddDays(-30.0)).Sum((Payment p) => p.Amount);
			decimal weeklyRevenue = allPayments.Where((Payment p) => p.Status == "Completed" && p.CreatedAtUtc >= DateTimeOffset.UtcNow.AddDays(-7.0)).Sum((Payment p) => p.Amount);
			var byProvider = (from p in allPayments
				where p.Status == "Completed"
				group p by p.Provider into g
				select new
				{
					provider = g.Key,
					total = g.Sum((Payment p) => p.Amount),
					count = g.Count()
				}).ToList();
			var last30Days = (from p in allPayments
				where p.Status == "Completed" && p.CreatedAtUtc >= DateTimeOffset.UtcNow.AddDays(-30.0)
				group p by p.CreatedAtUtc.Date.ToString("yyyy-MM-dd") into g
				select new
				{
					date = g.Key,
					amount = g.Sum((Payment p) => p.Amount)
				} into x
				orderby x.date
				select x).ToList();
			var userTiers = (from u in await (from u in db.Users.AsNoTracking()
					select u.SubscriptionTier).ToListAsync(ct)
				group u by u.ToString() into g
				select new
				{
					tier = g.Key,
					count = g.Count()
				}).ToList();
			return Results.Ok(new
			{
				totalRevenue = totalRevenue,
				monthlyRevenue = monthlyRevenue,
				weeklyRevenue = weeklyRevenue,
				totalTransactions = allPayments.Count((Payment p) => p.Status == "Completed"),
				byProvider = byProvider,
				last30Days = last30Days,
				userTiers = userTiers
			});
		}).WithName("AdminGetRevenue");
		endpoints.MapGet("/system-stats", (Func<PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			int totalUsers = await db.Users.CountAsync(ct);
			int totalVideos = await db.Videos.CountAsync(ct);
			int totalShorts = await db.Shorts.CountAsync(ct);
			int totalSchedules = await db.Schedules.CountAsync(ct);
			int totalPayments = await db.Payments.CountAsync(ct);
			int totalSocial = await db.SocialAccounts.CountAsync(ct);
			int newUsersToday = await db.Users.CountAsync((User u) => u.CreatedAtUtc >= (DateTimeOffset)DateTimeOffset.UtcNow.Date, ct);
			var videosByStatus = (from v in await (from v in db.Videos.AsNoTracking()
					select v.Status).ToListAsync(ct)
				group v by v.ToString() into g
				select new
				{
					status = g.Key,
					count = g.Count()
				}).ToList();
			var schedulesByStatus = (from s in await (from s in db.Schedules.AsNoTracking()
					select s.Status).ToListAsync(ct)
				group s by s.ToString() into g
				select new
				{
					status = g.Key,
					count = g.Count()
				}).ToList();
			return Results.Ok(new
			{
				totalUsers = totalUsers,
				totalVideos = totalVideos,
				totalShorts = totalShorts,
				totalSchedules = totalSchedules,
				totalPayments = totalPayments,
				totalSocial = totalSocial,
				newUsersToday = newUsersToday,
				videosByStatus = videosByStatus,
				schedulesByStatus = schedulesByStatus,
				serverTime = DateTimeOffset.UtcNow
			});
		}).WithName("AdminGetSystemStats");
		endpoints.MapGet("/audit-logs", (Func<int, int, string, string, PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(int page, int pageSize, string? action, string? userId, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			page = Math.Max(1, page);
			pageSize = Math.Clamp(pageSize, 5, 200);
			IQueryable<AuditLog> query = db.AuditLogs.AsNoTracking();
			if (!string.IsNullOrWhiteSpace(action))
			{
				query = query.Where((AuditLog a) => a.Action.Contains(action));
			}
			if (!string.IsNullOrWhiteSpace(userId) && Guid.TryParse(userId, out var uid))
			{
				query = query.Where((AuditLog a) => a.UserId == uid);
			}
			return Results.Ok(new
			{
				total = await query.CountAsync(ct),
				page = page,
				pageSize = pageSize,
				logs = await (from a in query.OrderByDescending((AuditLog a) => a.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize)
					select new AdminAuditLogDto(a.Id, a.UserId, a.Action, a.EntityName, a.EntityId, a.MetadataJson, a.CreatedAtUtc)).ToListAsync(ct)
			});
		}).WithName("AdminGetAuditLogs");
		endpoints.MapGet("/analytics", (Func<PublisherDbContext, ClaimsPrincipal, CancellationToken, Task<IResult>>)async delegate(PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
		{
			if (!IsAdmin(caller))
			{
				return Results.Forbid();
			}
			var userGrowth = (from d in await (from u in db.Users.AsNoTracking()
					where u.CreatedAtUtc >= DateTimeOffset.UtcNow.AddDays(-30.0)
					select u.CreatedAtUtc).ToListAsync(ct)
				group d by d.Date.ToString("yyyy-MM-dd") into g
				select new
				{
					date = g.Key,
					count = g.Count()
				} into x
				orderby x.date
				select x).ToList();
			var videoFunnel = (from v in await (from v in db.Videos.AsNoTracking()
					select v.Status).ToListAsync(ct)
				group v by v.ToString() into g
				select new
				{
					status = g.Key,
					count = g.Count()
				}).ToList();
			var rawSchedules = await (from s in db.Schedules.AsNoTracking()
				select new { s.Status, s.Platform }).ToListAsync(ct);
			var publishStats = (from s in rawSchedules
				group s by s.Status.ToString() into g
				select new
				{
					status = g.Key,
					count = g.Count()
				}).ToList();
			var platformBreakdown = (from s in rawSchedules
				group s by s.Platform.ToString() into g
				select new
				{
					platform = g.Key,
					count = g.Count()
				}).ToList();
			var socialBreakdown = (from sa in await (from sa in db.SocialAccounts.AsNoTracking()
					select sa.Platform).ToListAsync(ct)
				group sa by sa.ToString() into g
				select new
				{
					platform = g.Key,
					count = g.Count()
				}).ToList();
			return Results.Ok(new
			{
				activeUsersLast7Days = await db.Users.CountAsync((User u) => u.LastLoginAtUtc != null && u.LastLoginAtUtc >= DateTimeOffset.UtcNow.AddDays(-7.0), ct),
				userGrowthLast30Days = userGrowth,
				videoFunnel = videoFunnel,
				publishingSuccessRate = publishStats,
				platformBreakdown = platformBreakdown,
				socialBreakdown = socialBreakdown
			});
		}).WithName("AdminGetAnalytics");
		return app;
	}

	private static bool IsAdmin(ClaimsPrincipal user)
	{
		return user.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") == "Admin";
	}
}
