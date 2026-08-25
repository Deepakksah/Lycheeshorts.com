using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record SubmitVideoRequest(string OriginalFileUri, string? Title, TimeSpan? Duration);
