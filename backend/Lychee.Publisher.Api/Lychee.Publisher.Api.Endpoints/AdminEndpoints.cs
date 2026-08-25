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
        var endpoints = app.MapGroup("/api/v1/admin").RequireAuthorization().WithTags("Admin");

        // 1. Users List (In-memory sorting & paging for SQLite DateTimeOffset compatibility)
        endpoints.MapGet("/users", async (int? page, int? pageSize, string? search, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct) =>
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            int p = Math.Max(1, page ?? 1);
            int ps = Math.Clamp(pageSize ?? 10, 5, 100);

            IQueryable<User> query = db.Users.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(s) || (u.DisplayName != null && u.DisplayName.ToLower().Contains(s)));
            }

            var allMatching = await query.ToListAsync(ct);
            int total = allMatching.Count;
            var usersList = allMatching
                .OrderByDescending(u => u.CreatedAtUtc)
                .Skip((p - 1) * ps)
                .Take(ps)
                .Select(u => new AdminUserDto(
                    u.Id,
                    u.Email,
                    u.DisplayName,
                    u.Role,
                    u.SubscriptionTier.ToString(),
                    u.IsEmailVerified,
                    u.LastLoginAtUtc,
                    u.CreatedAtUtc
                ))
                .ToList();

            return Results.Ok(new
            {
                total,
                page = p,
                pageSize = ps,
                users = usersList
            });
        }).WithName("AdminGetUsers");

        // 2. Change Role
        endpoints.MapPatch("/users/{id:guid}/role", async (Guid id, ChangeRoleRequest request, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct) =>
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            var user = await db.Users.FindAsync([id], ct);
            if (user == null) return Results.NotFound();

            user.Role = request.Role;
            user.UpdatedAtUtc = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);

            return Results.Ok(new { userId = id, newRole = request.Role });
        }).WithName("AdminChangeUserRole");

        // 3. Change Tier
        endpoints.MapPatch("/users/{id:guid}/tier", async (Guid id, ChangeSubscriptionTierRequest request, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct) =>
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            var user = await db.Users.FindAsync([id], ct);
            if (user == null) return Results.NotFound();

            if (!Enum.TryParse<SubscriptionTier>(request.Tier, true, out var tier))
            {
                return Results.BadRequest("Invalid subscription tier.");
            }

            user.SubscriptionTier = tier;
            user.UpdatedAtUtc = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);

            return Results.Ok(new { userId = id, newTier = tier.ToString() });
        }).WithName("AdminChangeUserTier");

        // 4. Delete User
        endpoints.MapDelete("/users/{id:guid}", async (Guid id, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct) =>
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            var user = await db.Users.FindAsync([id], ct);
            if (user == null) return Results.NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync(ct);

            return Results.NoContent();
        }).WithName("AdminDeleteUser");

        // 5. Subscriptions List (In-memory sorting & paging for SQLite DateTimeOffset compatibility)
        endpoints.MapGet("/subscriptions", async (int? page, int? pageSize, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct) =>
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            int p = Math.Max(1, page ?? 1);
            int ps = Math.Clamp(pageSize ?? 10, 5, 100);

            var allPayments = await db.Payments.AsNoTracking()
                .Include(pay => pay.User)
                .ToListAsync(ct);

            int total = allPayments.Count;
            var rows = allPayments
                .OrderByDescending(pay => pay.CreatedAtUtc)
                .Skip((p - 1) * ps)
                .Take(ps)
                .Select(pay => new AdminSubscriptionDto(
                    pay.Id,
                    pay.User?.Email ?? "N/A",
                    pay.User?.DisplayName ?? "N/A",
                    pay.Provider,
                    pay.ProviderPaymentId,
                    pay.Amount,
                    pay.Currency,
                    pay.Status,
                    pay.CreatedAtUtc
                ))
                .ToList();

            return Results.Ok(new
            {
                total,
                page = p,
                pageSize = ps,
                rows
            });
        }).WithName("AdminGetSubscriptions");

        // 6. Revenue Stats
        endpoints.MapGet("/revenue", async (PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct) =>
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            var allPayments = await db.Payments.AsNoTracking().ToListAsync(ct);
            var completed = allPayments.Where(pay => pay.Status == "Completed").ToList();

            var cutoff30Days = DateTimeOffset.UtcNow.AddDays(-30);
            var cutoff7Days = DateTimeOffset.UtcNow.AddDays(-7);

            decimal totalRevenue = completed.Sum(pay => pay.Amount);
            decimal monthlyRevenue = completed.Where(pay => pay.CreatedAtUtc >= cutoff30Days).Sum(pay => pay.Amount);
            decimal weeklyRevenue = completed.Where(pay => pay.CreatedAtUtc >= cutoff7Days).Sum(pay => pay.Amount);

            var byProvider = completed
                .GroupBy(pay => pay.Provider)
                .Select(g => new { provider = g.Key, total = g.Sum(pay => pay.Amount), count = g.Count() })
                .ToList();

            var last30Days = completed
                .Where(pay => pay.CreatedAtUtc >= cutoff30Days)
                .GroupBy(pay => pay.CreatedAtUtc.Date.ToString("yyyy-MM-dd"))
                .Select(g => new { date = g.Key, amount = g.Sum(pay => pay.Amount) })
                .OrderBy(x => x.date)
                .ToList();

            var allUsers = await db.Users.AsNoTracking().Select(u => u.SubscriptionTier).ToListAsync(ct);
            var userTiers = allUsers
                .GroupBy(tier => tier.ToString())
                .Select(g => new { tier = g.Key, count = g.Count() })
                .ToList();

            return Results.Ok(new
            {
                totalRevenue,
                monthlyRevenue,
                weeklyRevenue,
                totalTransactions = completed.Count,
                byProvider,
                last30Days,
                userTiers
            });
        }).WithName("AdminGetRevenue");

        // 7. System Stats (Mapped to both /stats and /system-stats)
        async Task<IResult> GetSystemStatsHandler(PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct)
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            var todayStart = DateTimeOffset.UtcNow.Date;

            int totalUsers = await db.Users.CountAsync(ct);
            int totalVideos = await db.Videos.CountAsync(ct);
            int totalShorts = await db.Shorts.CountAsync(ct);
            int totalSchedules = await db.Schedules.CountAsync(ct);
            int totalPayments = await db.Payments.CountAsync(ct);
            int totalSocial = await db.SocialAccounts.CountAsync(ct);

            var userDates = await db.Users.AsNoTracking().Select(u => u.CreatedAtUtc).ToListAsync(ct);
            int newUsersToday = userDates.Count(d => d >= todayStart);

            var videoStatuses = await db.Videos.AsNoTracking().Select(v => v.Status).ToListAsync(ct);
            var videosByStatus = videoStatuses
                .GroupBy(s => s.ToString())
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToList();

            var scheduleStatuses = await db.Schedules.AsNoTracking().Select(s => s.Status).ToListAsync(ct);
            var schedulesByStatus = scheduleStatuses
                .GroupBy(s => s.ToString())
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToList();

            return Results.Ok(new
            {
                totalUsers,
                totalVideos,
                totalShorts,
                totalSchedules,
                totalPayments,
                totalSocial,
                newUsersToday,
                videosByStatus,
                schedulesByStatus,
                serverTime = DateTimeOffset.UtcNow
            });
        }

        endpoints.MapGet("/stats", GetSystemStatsHandler).WithName("AdminGetStats");
        endpoints.MapGet("/system-stats", GetSystemStatsHandler).WithName("AdminGetSystemStats");

        // 8. Audit Logs (In-memory sorting & paging for SQLite DateTimeOffset compatibility)
        endpoints.MapGet("/audit-logs", async (int? page, int? pageSize, string? action, string? userId, PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct) =>
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            int p = Math.Max(1, page ?? 1);
            int ps = Math.Clamp(pageSize ?? 10, 5, 200);

            IQueryable<AuditLog> query = db.AuditLogs.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action))
            {
                var aLower = action.Trim().ToLower();
                query = query.Where(a => a.Action.ToLower().Contains(aLower));
            }
            if (!string.IsNullOrWhiteSpace(userId) && Guid.TryParse(userId, out var uid))
            {
                query = query.Where(a => a.UserId == uid);
            }

            var allLogs = await query.ToListAsync(ct);
            int total = allLogs.Count;
            var logDtos = allLogs
                .OrderByDescending(a => a.CreatedAtUtc)
                .Skip((p - 1) * ps)
                .Take(ps)
                .Select(a => new AdminAuditLogDto(
                    a.Id,
                    a.UserId,
                    a.Action,
                    a.EntityName,
                    a.EntityId,
                    a.MetadataJson,
                    a.CreatedAtUtc
                ))
                .ToList();

            return Results.Ok(new
            {
                total,
                page = p,
                pageSize = ps,
                logs = logDtos
            });
        }).WithName("AdminGetAuditLogs");

        // 9. Analytics (In-memory for SQLite DateTimeOffset compatibility)
        endpoints.MapGet("/analytics", async (PublisherDbContext db, ClaimsPrincipal caller, CancellationToken ct) =>
        {
            if (!IsAdmin(caller)) return Results.Forbid();

            var cutoff30Days = DateTimeOffset.UtcNow.AddDays(-30);
            var cutoff7Days = DateTimeOffset.UtcNow.AddDays(-7);

            var allUsers = await db.Users.AsNoTracking()
                .Select(u => new { u.CreatedAtUtc, u.LastLoginAtUtc })
                .ToListAsync(ct);

            var userGrowth = allUsers
                .Where(u => u.CreatedAtUtc >= cutoff30Days)
                .GroupBy(u => u.CreatedAtUtc.Date.ToString("yyyy-MM-dd"))
                .Select(g => new { date = g.Key, count = g.Count() })
                .OrderBy(x => x.date)
                .ToList();

            var videoStatuses = await db.Videos.AsNoTracking().Select(v => v.Status).ToListAsync(ct);
            var videoFunnel = videoStatuses
                .GroupBy(v => v.ToString())
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToList();

            var rawSchedules = await db.Schedules.AsNoTracking()
                .Select(s => new { s.Status, s.Platform })
                .ToListAsync(ct);

            var publishStats = rawSchedules
                .GroupBy(s => s.Status.ToString())
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToList();

            var platformBreakdown = rawSchedules
                .GroupBy(s => s.Platform.ToString())
                .Select(g => new { platform = g.Key, count = g.Count() })
                .ToList();

            var socialAccounts = await db.SocialAccounts.AsNoTracking().Select(sa => sa.Platform).ToListAsync(ct);
            var socialBreakdown = socialAccounts
                .GroupBy(sa => sa.ToString())
                .Select(g => new { platform = g.Key, count = g.Count() })
                .ToList();

            int active7Days = allUsers.Count(u => u.LastLoginAtUtc != null && u.LastLoginAtUtc >= cutoff7Days);

            return Results.Ok(new
            {
                activeUsersLast7Days = active7Days,
                userGrowthLast30Days = userGrowth,
                videoFunnel,
                publishingSuccessRate = publishStats,
                platformBreakdown,
                socialBreakdown
            });
        }).WithName("AdminGetAnalytics");

        return app;
    }

    private static bool IsAdmin(ClaimsPrincipal user)
    {
        return user.IsInRole("Admin") ||
               user.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role") == "Admin" ||
               user.FindFirstValue(ClaimTypes.Role) == "Admin" ||
               user.FindFirstValue("role") == "Admin";
    }
}
