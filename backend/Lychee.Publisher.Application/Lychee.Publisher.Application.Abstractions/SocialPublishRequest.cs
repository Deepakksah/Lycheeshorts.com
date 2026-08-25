using System;
using System.Collections.Generic;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record SocialPublishRequest(Guid ShortClipId, Guid SocialAccountId, PlatformType Platform, string VideoUri, string Title, string Description, IReadOnlyCollection<string> Hashtags);
