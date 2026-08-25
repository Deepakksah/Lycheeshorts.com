using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;
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

public static class GeminiEndpoints
{
    public static void MapGeminiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/gemini")
            .WithTags("Gemini AI Studio")
            .RequireAuthorization();

        // 1. Get Available Models
        group.MapGet("/models", () =>
        {
            var models = new[]
            {
                new { id = "gemini-3-preview", name = "Gemini 3.0 Preview", category = "Flagship Reasoning", badge = "Next-Gen AI", isFree = true },
                new { id = "veo-3.1-video", name = "Google Veo 3.1 Video", category = "Cinematic Text-to-Video", badge = "Veo 3.1 Engine", isFree = true },
                new { id = "notebooklm-audio", name = "NotebookLM Deep Script", category = "Source & Deep Reasoning", badge = "Podcast & Dialogue", isFree = true },
                new { id = "workspace-ai-agent", name = "Workspace AI Agent", category = "Multi-Channel Distribution", badge = "Auto-Publish", isFree = true },
                new { id = "gemini-nano-edge", name = "Gemini Nano (Edge)", category = "On-Device Zero-Latency", badge = "Instant Speed", isFree = true },
                new { id = "gemini-2.0-flash", name = "Gemini 2.0 Flash", category = "High-Speed Multimodal", badge = "Recommended", isFree = true },
                new { id = "gemini-1.5-pro", name = "Gemini 1.5 Pro", category = "Long-Context Pro", badge = "2M Context", isFree = true }
            };
            return Results.Ok(models);
        });

        // 2. Publish Generated Gemini Short directly to Workspace
        group.MapPost("/publish", async (
            GeminiPublishRequest req,
            ClaimsPrincipal principal,
            PublisherDbContext db,
            CancellationToken ct) =>
        {
            var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                return Results.Unauthorized();
            }

            var title = string.IsNullOrWhiteSpace(req.Title) ? "Untitled Gemini Short" : req.Title.Trim();
            var duration = req.DurationSeconds > 0 ? req.DurationSeconds : 36;
            
            var video = new Video
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                SourceType = "AI_Generated",
                SourceUrl = $"https://aistudio.google.com/video/{Guid.NewGuid()}",
                OriginalFileUri = $"/uploads/gemini/{Guid.NewGuid()}.mp4",
                ThumbnailUri = null,
                Duration = TimeSpan.FromSeconds(duration),
                Status = ProcessingStatus.Processed,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            db.Videos.Add(video);

            // Create a short clip entity for this video so it can immediately be scheduled or downloaded
            var shortClip = new ShortClip
            {
                Id = Guid.NewGuid(),
                VideoId = video.Id,
                Title = $"[Short] {title}",
                Description = req.Description ?? $"AI Video Short generated with {req.Model}",
                Hashtags = req.Hashtags != null ? string.Join(" ", req.Hashtags) : "#Shorts #Viral #LycheeAI",
                OutputUri = video.OriginalFileUri,
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.FromSeconds(duration),
                ViralityScore = (decimal)(req.ViralityScore > 0 ? req.ViralityScore : 95),
                Status = ProcessingStatus.Processed,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            db.Shorts.Add(shortClip);
            await db.SaveChangesAsync(ct);

            return Results.Ok(new
            {
                id = video.Id,
                userId = video.UserId,
                title = video.Title,
                originalFileUri = video.OriginalFileUri,
                sourceType = video.SourceType,
                status = video.Status.ToString(),
                createdAtUtc = video.CreatedAtUtc,
                shortClipId = shortClip.Id
            });
        });
    }
}

public class GeminiPublishRequest
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("durationSeconds")]
    public double DurationSeconds { get; set; } = 36;

    [JsonPropertyName("viralityScore")]
    public double ViralityScore { get; set; } = 95;

    [JsonPropertyName("hook")]
    public string? Hook { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("hashtags")]
    public List<string>? Hashtags { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; } = "gemini-3-preview";
}
