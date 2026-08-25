using System;

namespace Lychee.Publisher.Application.Features.Videos;

public sealed record SubmitVideoCommand(Guid UserId, string SourceType, string? SourceUrl, string OriginalFileUri, string? Title);
