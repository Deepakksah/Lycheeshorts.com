using System.Collections.Generic;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record GeneratedShortMetadata(string Title, string Description, IReadOnlyCollection<string> Hashtags, IReadOnlyCollection<string> SeoKeywords);
