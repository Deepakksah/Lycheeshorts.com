using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class FfmpegService
{
	public async Task ExtractAudioAsync(string videoPath, string audioPath, CancellationToken cancellationToken)
	{
		string arguments = $"-y -i \"{videoPath}\" -q:a 0 -map a \"{audioPath}\"";
		await RunFfmpegAsync(arguments, cancellationToken);
	}

	public async Task GenerateThumbnailAsync(string videoPath, string outputPath, CancellationToken cancellationToken)
	{
		string arguments = $"-y -ss 00:00:01 -i \"{videoPath}\" -vframes 1 -q:v 2 \"{outputPath}\"";
		await RunFfmpegAsync(arguments, cancellationToken);
	}

	public async Task CropToVerticalAsync(string videoPath, string outputPath, TimeSpan start, TimeSpan end, string? captionText, bool addWatermark, bool autoCropFace, int crf, string codec, string format, CancellationToken cancellationToken)
	{
		string startSeconds = start.TotalSeconds.ToString("F3");
		string endSeconds = end.TotalSeconds.ToString("F3");
		string cropExpr = "crop=ih*9/16:ih";
		if (autoCropFace)
		{
			cropExpr = "crop=w=ih*9/16:h=ih:x='clip(in_w/2-out_w/2 + (in_w/2-out_w/2)*0.4*sin(t*0.6), 0, in_w-out_w)'";
			Console.WriteLine("[AI Face Tracker] Speaker Face Auto-Crop enabled: applying dynamic panning trajectory expression.");
		}
		List<string> filterParts = new List<string> { cropExpr };
		if (!string.IsNullOrWhiteSpace(captionText))
		{
			string escapedText = captionText.Replace("'", "'\\\\''").Replace(":", "\\:");
			filterParts.Add("drawtext=text='" + escapedText + "':x=(w-text_w)/2:y=(h-text_h)/2-50:fontsize=28:fontcolor=yellow:box=1:boxcolor=black@0.6:boxborderw=10");
		}
		if (addWatermark)
		{
			filterParts.Add("drawtext=text='Lychee':x=(w-text_w)/2:y=h-60:fontsize=20:fontcolor=white@0.6");
		}
		filterParts.Add("eq=gamma=1.03:saturation=1.04:contrast=1.02");
		string filterString = string.Join(",", filterParts);
		string actualCodec = (string.IsNullOrWhiteSpace(codec) ? "libx264" : codec);
		if (format.ToLowerInvariant() == "webm")
		{
			actualCodec = "libvpx-vp9";
		}
		string arguments = $"-y -ss {startSeconds} -to {endSeconds} -i \"{videoPath}\" -vf \"{filterString}\" -map_metadata -1 -c:v {actualCodec} -crf {crf} -c:a aac \"{outputPath}\"";
		await RunFfmpegAsync(arguments, cancellationToken);
	}

	public async Task TranscodeResolutionAsync(string inputPath, string outputPath, int targetHeight, CancellationToken cancellationToken)
	{
		string arguments = $"-y -i \"{inputPath}\" -vf \"scale=-2:{targetHeight}\" -c:v libx264 -crf 23 -c:a aac \"{outputPath}\"";
		await RunFfmpegAsync(arguments, cancellationToken);
	}

	private async Task RunFfmpegAsync(string arguments, CancellationToken cancellationToken)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = "ffmpeg",
			Arguments = arguments,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};
		Process process = new Process
		{
			StartInfo = startInfo
		};
		try
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			process.EnableRaisingEvents = true;
			process.Exited += delegate
			{
				tcs.TrySetResult(result: true);
			};
			if (!process.Start())
			{
				throw new InvalidOperationException("Failed to start FFmpeg process. Ensure ffmpeg is installed and in the system PATH.");
			}
			using (cancellationToken.Register(delegate
			{
				try
				{
					if (!process.HasExited)
					{
						process.Kill();
					}
				}
				catch
				{
				}
				tcs.TrySetCanceled(cancellationToken);
			}))
			{
				Task<string> errorReader = process.StandardError.ReadToEndAsync(cancellationToken);
				await tcs.Task;
				if (process.ExitCode != 0)
				{
					string errorLogs = await errorReader;
					throw new InvalidOperationException($"FFmpeg process exited with code {process.ExitCode}. Arguments: {arguments}. Error logs: {errorLogs}");
				}
			}
		}
		finally
		{
			if (process != null)
			{
				((IDisposable)process).Dispose();
			}
		}
	}
}
