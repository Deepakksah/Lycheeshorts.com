namespace Lychee.Publisher.Application.Abstractions;

public sealed record RegisterRequest(string Email, string Password, string? DisplayName);
