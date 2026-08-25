using System.Text.Json.Serialization;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed record ShortSegmentCandidate([property: JsonPropertyName("startTimeSeconds")] double StartTimeSeconds, [property: JsonPropertyName("endTimeSeconds")] double EndTimeSeconds, [property: JsonPropertyName("viralityScore")] decimal ViralityScore, [property: JsonPropertyName("title")] string Title, [property: JsonPropertyName("description")] string Description, [property: JsonPropertyName("hashtags")] string Hashtags);
