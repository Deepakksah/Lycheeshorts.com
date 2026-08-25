using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record CreateCheckoutRequest(Guid PlanId, string Provider, string BillingCycle, string SuccessUrl, string CancelUrl);
