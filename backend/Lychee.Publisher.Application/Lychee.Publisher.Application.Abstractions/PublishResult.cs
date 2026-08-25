namespace Lychee.Publisher.Application.Abstractions;

public sealed record PublishResult(bool Succeeded, string? ExternalPostId, string? ErrorMessage);
