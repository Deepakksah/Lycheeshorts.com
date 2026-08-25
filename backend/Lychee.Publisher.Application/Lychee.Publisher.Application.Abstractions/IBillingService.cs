using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lychee.Publisher.Application.Abstractions;

public interface IBillingService
{
	Task<IEnumerable<SubscriptionPlanResponse>> GetSubscriptionPlansAsync(CancellationToken cancellationToken);

	Task<CheckoutResponse> CreateCheckoutSessionAsync(Guid userId, CreateCheckoutRequest request, CancellationToken cancellationToken);

	Task<bool> ProcessStripeWebhookAsync(string jsonPayload, string signatureHeader, CancellationToken cancellationToken);

	Task<bool> ProcessRazorpayWebhookAsync(string jsonPayload, string signatureHeader, CancellationToken cancellationToken);

	Task<IEnumerable<PaymentResponse>> GetUserPaymentHistoryAsync(Guid userId, CancellationToken cancellationToken);

	Task<UserUsageResponse> GetUserUsageAsync(Guid userId, CancellationToken cancellationToken);

	Task<byte[]> GenerateInvoicePdfAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken);
}
