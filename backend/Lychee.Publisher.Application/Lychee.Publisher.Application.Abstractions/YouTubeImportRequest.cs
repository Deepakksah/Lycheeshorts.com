using System.Text.Json.Serialization;

namespace Lychee.Publisher.Application.Abstractions;

public sealed class YouTubeImportRequest
{
    [JsonPropertyName("sourceUrl")]
    public string? SourceUrl { get; set; }

    [JsonPropertyName("youtubeUrl")]
    public string? YoutubeUrl { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    public string GetEffectiveUrl() => !string.IsNullOrWhiteSpace(SourceUrl) ? SourceUrl : (YoutubeUrl ?? string.Empty);
}
