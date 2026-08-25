using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record AuthResponse(Guid UserId, string Email, string DisplayName, string Role, string AccessToken, DateTimeOffset AccessTokenExpiresAtUtc, string RefreshToken, DateTimeOffset RefreshTokenExpiresAtUtc);
