namespace Lychee.Publisher.Api.Endpoints;

public sealed record ResetPasswordRequest(string Token, string NewPassword);
