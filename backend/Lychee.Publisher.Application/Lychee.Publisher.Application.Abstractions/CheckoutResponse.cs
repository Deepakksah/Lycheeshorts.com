namespace Lychee.Publisher.Application.Abstractions;

public sealed record CheckoutResponse(string SessionId, string PaymentUrl, string? OrderId);
