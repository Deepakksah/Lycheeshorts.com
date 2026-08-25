using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record PaymentResponse(Guid Id, string Provider, string ProviderPaymentId, decimal Amount, string Currency, string Status, DateTimeOffset CreatedAtUtc);
