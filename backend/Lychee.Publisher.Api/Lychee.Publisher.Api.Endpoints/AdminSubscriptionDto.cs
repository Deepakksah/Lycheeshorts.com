using System;

namespace Lychee.Publisher.Api.Endpoints;

public record AdminSubscriptionDto(Guid Id, string UserEmail, string? UserDisplayName, string Provider, string ProviderPaymentId, decimal Amount, string Currency, string Status, DateTimeOffset CreatedAtUtc);
