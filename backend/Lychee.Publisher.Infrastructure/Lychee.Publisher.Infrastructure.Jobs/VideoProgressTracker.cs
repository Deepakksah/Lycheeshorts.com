using System;
using System.Collections.Concurrent;

namespace Lychee.Publisher.Infrastructure.Jobs;

/// <summary>
/// In-memory store for real-time video processing progress.
/// Keyed by VideoId.
/// </summary>
public static class VideoProgressTracker
{
    private static readonly ConcurrentDictionary<Guid, VideoProgress> _progress = new();

    public static void Update(Guid videoId, int percent, string step)
    {
        _progress[videoId] = new VideoProgress(percent, step, DateTime.UtcNow);
    }

    public static VideoProgress? Get(Guid videoId)
    {
        return _progress.TryGetValue(videoId, out var p) ? p : null;
    }

    public static void Remove(Guid videoId)
    {
        _progress.TryRemove(videoId, out _);
    }
}

public sealed record VideoProgress(int Percent, string Step, DateTime UpdatedAt);
