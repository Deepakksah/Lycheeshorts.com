using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record RefreshTokenResult(string Token, string TokenHash, DateTimeOffset ExpiresAtUtc);
