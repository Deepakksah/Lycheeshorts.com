using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Hangfire;
using Lychee.Publisher.Api.Endpoints;
using Lychee.Publisher.Application;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Infrastructure;
using Lychee.Publisher.Infrastructure.Auth;
using Lychee.Publisher.Infrastructure.Jobs;
using Lychee.Publisher.Infrastructure.Persistence;
using Lychee.Publisher.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*");
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context => 
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            }));

    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests. Please slow down and try again shortly.",
            retryAfterSeconds = 60
        }, token);
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PublisherDbContext>();
    var hasher = scope.ServiceProvider.GetService<IPasswordHasher<User>>();
    await dbContext.Database.EnsureCreatedAsync();
    await DbSeeder.SeedAsync(dbContext, hasher);
}
catch (Exception ex)
{
    FileLogger.LogBackendError("DATABASE_INIT_ERROR", "Failed to migrate or seed database: " + ex.Message, ex);
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
        if (context.Response.StatusCode >= 400)
        {
            FileLogger.LogBackendError("HTTP_RESPONSE_ERROR", $"[HTTP {context.Response.StatusCode}] {context.Request.Method} {context.Request.Path}");
        }
    }
    catch (Exception ex)
    {
        FileLogger.LogBackendError(ex: ex, source: "UNHANDLED_EXCEPTION", message: $"[HTTP EXCEPTION] {context.Request.Method} {context.Request.Path}");
        throw;
    }
});

app.UseDeveloperExceptionPage();
app.UseExceptionHandler();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();

app.MapGet("/api/v1/health", () => Results.Ok(new
{
    service = "Lychee Publisher API",
    status = "Healthy",
    utc = DateTimeOffset.UtcNow
})).WithName("HealthCheck");

app.MapPost("/api/v1/logs/client-error", (ClientErrorLogRequest req) =>
{
    FileLogger.LogFrontendError(req.Message, req.Stack, req.Route);
    return Results.Ok();
}).AllowAnonymous().WithName("LogClientError");

app.MapPost("/api/v1/videos/preview-processing", async (VideoProcessingRequest request, IVideoProcessingService videoProcessingService, CancellationToken cancellationToken) => 
    Results.Accepted(value: await videoProcessingService.GenerateShortsAsync(request, cancellationToken), uri: $"/api/v1/videos/{request.VideoId}/shorts")
).WithName("PreviewVideoProcessing");

app.MapAuthEndpoints();
app.MapVideoEndpoints();
app.MapScheduleEndpoints();
app.MapPaymentEndpoints();
app.MapSocialAccountEndpoints();
app.MapAdminEndpoints();
app.MapGeminiEndpoints();

try
{
    RecurringJob.AddOrUpdate<PublishScheduledClipsJob>("publish-scheduled-clips", job => job.PublishPendingClipsAsync(CancellationToken.None), Cron.Minutely());
}
catch (Exception ex)
{
    FileLogger.LogBackendError("HANGFIRE_JOB_INIT", "Failed to register recurring job: " + ex.Message, ex);
}

app.Run();
