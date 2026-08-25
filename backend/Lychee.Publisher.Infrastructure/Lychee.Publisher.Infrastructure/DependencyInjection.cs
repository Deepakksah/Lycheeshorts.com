using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Infrastructure.Auth;
using Lychee.Publisher.Infrastructure.Jobs;
using Lychee.Publisher.Infrastructure.Persistence;
using Lychee.Publisher.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lychee.Publisher.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        // Use SQLite when connection string is a SQLite Data Source, otherwise SQL Server
        if (connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<PublisherDbContext>(options =>
                options.UseSqlite(connectionString)
                       .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
        }
        else
        {
            services.AddDbContext<PublisherDbContext>(options =>
                options.UseSqlServer(connectionString)
                       .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
        }

        // Use in-memory Hangfire storage for SQLite/dev environments (no SQL Server needed)
        if (connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        {
            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseMemoryStorage());
        }
        else
        {
            // Try SQL Server storage; fall back to in-memory if SQL Server is unavailable (e.g., dev without SQL Server)
            bool hangfireSqlOk = false;
            try
            {
                using var testConn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                testConn.Open();
                hangfireSqlOk = true;
            }
            catch
            {
                hangfireSqlOk = false;
            }

            if (hangfireSqlOk)
            {
                services.AddHangfire(config =>
                    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                          .UseSimpleAssemblyNameTypeSerializer()
                          .UseRecommendedSerializerSettings()
                          .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions
                          {
                              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                              QueuePollInterval = TimeSpan.Zero,
                              UseRecommendedIsolationLevel = true,
                              DisableGlobalLocks = true
                          }));
            }
            else
            {
                // SQL Server not reachable — use in-memory Hangfire (safe for local dev)
                services.AddHangfire(config =>
                    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                          .UseSimpleAssemblyNameTypeSerializer()
                          .UseRecommendedSerializerSettings()
                          .UseMemoryStorage());
            }
        }

        services.AddHangfireServer();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAiContentService, AiContentService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IVideoProcessingService, VideoProcessingService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<ISchedulingService, SchedulingService>();
        services.AddScoped<ISocialPublisher, SocialPublisher>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddSingleton<IDataEncryptionService, DataEncryptionService>();
        services.Configure<OpenAiOptions>(configuration.GetSection("OpenAi"));
        services.AddSingleton<FfmpegService>();
        services.AddHttpClient<OpenAiService>();
        services.AddTransient<VideoProcessingJob>();
        services.AddTransient<PublishScheduledClipsJob>();

        return services;
    }
}
