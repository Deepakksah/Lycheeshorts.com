using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
	private const long MaxVideoSizeBytes = 524288000L;

	private static readonly HashSet<string> AllowedVideoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mp4", ".mov", ".m4v", ".webm", ".mkv" };

	public async Task<StoredFileResponse> SaveVideoAsync(Guid userId, string originalFileName, string? contentType, long length, Stream content, CancellationToken cancellationToken)
	{
		if (length <= 0)
		{
			throw new InvalidOperationException("Uploaded video is empty.");
		}
		if (length > 524288000)
		{
			throw new InvalidOperationException("Uploaded video exceeds the 500 MB limit.");
		}
		string extension = Path.GetExtension(originalFileName);
		if (string.IsNullOrWhiteSpace(extension) || !AllowedVideoExtensions.Contains(extension))
		{
			throw new InvalidOperationException("Unsupported video format. Upload MP4, MOV, M4V, WEBM, or MKV.");
		}
		string webRootPath = environment.WebRootPath;
		if (string.IsNullOrWhiteSpace(webRootPath))
		{
			webRootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
		}
		string relativeDirectory = Path.Combine("uploads", "videos", userId.ToString("N"));
		string physicalDirectory = Path.Combine(webRootPath, relativeDirectory);
		Directory.CreateDirectory(physicalDirectory);
		string storedFileName = $"{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
		string physicalPath = Path.Combine(physicalDirectory, storedFileName);
		StoredFileResponse result;
		await using (FileStream output = File.Create(physicalPath))
		{
			await content.CopyToAsync(output, cancellationToken);
			string relativeUri = Path.Combine(relativeDirectory, storedFileName).Replace('\\', '/');
			result = new StoredFileResponse(relativeUri, storedFileName, contentType, length);
		}
		return result;
	}
}
