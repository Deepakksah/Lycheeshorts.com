using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record AccessTokenResult(string Token, DateTimeOffset ExpiresAtUtc);
