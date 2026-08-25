using System;
using Lychee.Publisher.Domain.Enums;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record CreateScheduleRequest(Guid ShortClipId, Guid SocialAccountId, PlatformType Platform, DateTimeOffset PublishAtUtc);
