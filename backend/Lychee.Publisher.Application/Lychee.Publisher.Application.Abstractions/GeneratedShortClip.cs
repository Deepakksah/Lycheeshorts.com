using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record GeneratedShortClip(string OutputUri, TimeSpan StartTime, TimeSpan EndTime, decimal ViralityScore, string CaptionsJson);
