using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record UpdateScheduleRequest(DateTimeOffset PublishAtUtc);
