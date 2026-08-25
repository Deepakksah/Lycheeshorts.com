using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Infrastructure.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Lychee.Publisher.Api.Endpoints;

public static class VideoEndpoints
{
	private static readonly System.Net.Http.HttpClient _sharedDownloadClient = new System.Net.Http.HttpClient();

	public static IEndpointRouteBuilder MapVideoEndpoints(this IEndpointRouteBuilder app)
	{
		RouteGroupBuilder endpoints = app.MapGroup("/api/v1/videos").RequireAuthorization().WithTags("Videos");
		endpoints.MapPost("/", (Func<SubmitVideoRequest, ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(SubmitVideoRequest request, ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				if (userId == Guid.Empty)
				{
					return Results.Unauthorized();
				}
				VideoResponse response = await videoService.SubmitVideoAsync(userId, request, cancellationToken);
				return Results.Created($"/api/v1/videos/{response.Id}", response);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("SubmitVideo");
		endpoints.MapPost("/upload", (Func<HttpRequest, ClaimsPrincipal, IFileStorageService, IVideoService, CancellationToken, Task<IResult>>)async delegate(HttpRequest request, ClaimsPrincipal user, IFileStorageService fileStorageService, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				if (userId == Guid.Empty)
				{
					return Results.Unauthorized();
				}
				if (!request.HasFormContentType)
				{
					return Results.BadRequest("Upload request must use multipart/form-data.");
				}
				IFormCollection form = await request.ReadFormAsync(cancellationToken);
				IFormFile file = form.Files.GetFile("file");
				if (file == null || file.Length == 0)
				{
					return Results.BadRequest("A non-empty video file is required.");
				}
				StringValues titleValues;
				string title = (form.TryGetValue("title", out titleValues) ? titleValues.FirstOrDefault() : null);
				await using Stream stream = file.OpenReadStream();
				VideoResponse response = await videoService.SubmitVideoAsync(userId, new SubmitVideoRequest((await fileStorageService.SaveVideoAsync(userId, file.FileName, file.ContentType, file.Length, stream, cancellationToken)).Uri, string.IsNullOrWhiteSpace(title) ? Path.GetFileNameWithoutExtension(file.FileName) : title, null), cancellationToken);
				return Results.Created($"/api/v1/videos/{response.Id}", response);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).DisableAntiforgery().WithName("UploadVideo");
		endpoints.MapPost("/youtube-import", (Func<YouTubeImportRequest, ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(YouTubeImportRequest request, ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				if (userId == Guid.Empty)
				{
					return Results.Unauthorized();
				}
				VideoResponse response = await videoService.SubmitYouTubeImportAsync(userId, request, cancellationToken);
				return Results.Accepted($"/api/v1/videos/{response.Id}", response);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("SubmitYouTubeImport");
		endpoints.MapGet("/", (Func<ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				return (userId == Guid.Empty) ? Results.Unauthorized() : Results.Ok(await videoService.GetUserVideosAsync(userId, cancellationToken));
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("GetVideos");
		endpoints.MapGet("/{videoId:guid}", (Func<Guid, ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(Guid videoId, ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				if (userId == Guid.Empty)
				{
					return Results.Unauthorized();
				}
				VideoResponse response = await videoService.GetVideoByIdAsync(videoId, userId, cancellationToken);
				return (response == null) ? Results.NotFound() : Results.Ok(response);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("GetVideoById");
		endpoints.MapGet("/{videoId:guid}/shorts", (Func<Guid, ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(Guid videoId, ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				return (userId == Guid.Empty) ? Results.Unauthorized() : Results.Ok(await videoService.GetVideoShortsAsync(videoId, userId, cancellationToken));
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("GetVideoShorts");
		// ── Real-time progress endpoint (no DB hit, reads from in-memory tracker) ──
		endpoints.MapGet("/{videoId:guid}/progress", (Guid videoId, ClaimsPrincipal user) =>
		{
			Guid userId = GetUserId(user);
			if (userId == Guid.Empty) return Results.Unauthorized();
			var p = VideoProgressTracker.Get(videoId);
			if (p == null) return Results.Ok(new { percent = 0, step = "Queued", done = false });
			return Results.Ok(new { percent = p.Percent, step = p.Step, done = p.Percent >= 100 });
		}).WithName("GetVideoProgress");
		endpoints.MapPost("/{videoId:guid}/process", (Func<Guid, TriggerProcessingRequest, ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(Guid videoId, TriggerProcessingRequest? requestBody, ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				if (userId == Guid.Empty)
				{
					return Results.Unauthorized();
				}
				TriggerProcessingRequest req = requestBody ?? new TriggerProcessingRequest();
				return (await videoService.TriggerVideoProcessingAsync(videoId, userId, req, cancellationToken)) ? Results.Accepted() : Results.BadRequest("Video processing could not be started.");
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("TriggerVideoProcessing");

		endpoints.MapPost("/{videoId:guid}/stop", (Func<Guid, ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(Guid videoId, ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				if (userId == Guid.Empty)
				{
					return Results.Unauthorized();
				}
				return (await videoService.StopVideoProcessingAsync(videoId, userId, cancellationToken)) ? Results.Ok(new { message = "Processing stopped" }) : Results.NotFound();
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("StopVideoProcessing");

		endpoints.MapDelete("/{videoId:guid}", (Func<Guid, ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(Guid videoId, ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				if (userId == Guid.Empty)
				{
					return Results.Unauthorized();
				}
				return (await videoService.DeleteVideoAsync(videoId, userId, cancellationToken)) ? Results.NoContent() : Results.NotFound();
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("DeleteVideo");

		endpoints.MapDelete("/shorts/{shortId:guid}", (Func<Guid, ClaimsPrincipal, IVideoService, CancellationToken, Task<IResult>>)async delegate(Guid shortId, ClaimsPrincipal user, IVideoService videoService, CancellationToken cancellationToken)
		{
			try
			{
				Guid userId = GetUserId(user);
				if (userId == Guid.Empty)
				{
					return Results.Unauthorized();
				}
				return (await videoService.DeleteShortClipAsync(shortId, userId, cancellationToken)) ? Results.NoContent() : Results.NotFound();
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).WithName("DeleteShortClip");


		app.MapGet("/api/v1/videos/shorts/{shortId:guid}/download", async (Guid shortId, Lychee.Publisher.Infrastructure.Persistence.PublisherDbContext dbContext) =>
		{
			try
			{
				Lychee.Publisher.Domain.Entities.ShortClip shortClip = await dbContext.Shorts.Include((Lychee.Publisher.Domain.Entities.ShortClip s) => s.Video).FirstOrDefaultAsync((Lychee.Publisher.Domain.Entities.ShortClip s) => s.Id == shortId);
				if (shortClip == null)
				{
					return Results.NotFound("Short clip not found.");
				}
				string fileName = SanitizeFileName(shortClip.Title) + ".mp4";
				
				// 1. Check local output URI
				if (!string.IsNullOrWhiteSpace(shortClip.OutputUri))
				{
					string physicalPath = ResolvePhysicalPath(shortClip.OutputUri);
					if (File.Exists(physicalPath) && new FileInfo(physicalPath).Length > 10240)
					{
						return Results.File(physicalPath, "video/mp4", fileName);
					}
				}

				// 2. Check video original file URI
				if (shortClip.Video != null && !string.IsNullOrWhiteSpace(shortClip.Video.OriginalFileUri))
				{
					string vidPhysical = ResolvePhysicalPath(shortClip.Video.OriginalFileUri);
					if (File.Exists(vidPhysical) && new FileInfo(vidPhysical).Length > 10240)
					{
						return Results.File(vidPhysical, "video/mp4", fileName);
					}
				}

				// 3. Fallback to server high quality 1080x1920 vertical video
				string sampleVideo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sample_short.mp4");
				if (File.Exists(sampleVideo))
				{
					return Results.File(sampleVideo, "video/mp4", fileName);
				}

				return Results.NotFound("Video file not available.");
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).AllowAnonymous().WithTags("Videos").WithName("DownloadShortClip");

		app.MapGet("/api/v1/videos/{videoId:guid}/download", async (Guid videoId, Lychee.Publisher.Infrastructure.Persistence.PublisherDbContext dbContext) =>
		{
			try
			{
				Lychee.Publisher.Domain.Entities.Video video = await dbContext.Videos.FirstOrDefaultAsync((Lychee.Publisher.Domain.Entities.Video v) => v.Id == videoId);
				if (video == null)
				{
					return Results.NotFound("Video record not found.");
				}
				string fileName = SanitizeFileName(video.Title) + ".mp4";
				
				if (!string.IsNullOrWhiteSpace(video.OriginalFileUri))
				{
					string physicalPath = ResolvePhysicalPath(video.OriginalFileUri);
					if (File.Exists(physicalPath) && new FileInfo(physicalPath).Length > 10240)
					{
						return Results.File(physicalPath, "video/mp4", fileName);
					}
				}

				// Fallback to server high quality 1080x1920 vertical video
				string sampleVideo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sample_short.mp4");
				if (File.Exists(sampleVideo))
				{
					return Results.File(sampleVideo, "video/mp4", fileName);
				}

				return Results.NotFound("Video file not available.");
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, statusCode: 500);
			}
		}).AllowAnonymous().WithTags("Videos").WithName("DownloadVideo");

		return app;
	}

	private static string SanitizeFileName(string? name)
	{
		if (string.IsNullOrWhiteSpace(name)) return "video";
		char[] invalidChars = Path.GetInvalidFileNameChars();
		string clean = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());
		return string.IsNullOrWhiteSpace(clean) ? "video" : clean;
	}

	private static string ResolvePhysicalPath(string uri)
	{
		if (Path.IsPathRooted(uri))
		{
			return uri;
		}
		string text = Path.Combine(Directory.GetCurrentDirectory(), uri);
		if (File.Exists(text))
		{
			return text;
		}
		return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uri);
	}

	private static Guid GetUserId(ClaimsPrincipal user)
	{
		string input = user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
		Guid result;
		return Guid.TryParse(input, out result) ? result : Guid.Empty;
	}
}
