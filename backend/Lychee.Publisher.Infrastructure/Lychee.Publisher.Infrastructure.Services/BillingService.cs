using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Domain.Enums;
using Lychee.Publisher.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class BillingService : IBillingService
{
	private sealed class RazorpayOrderResponse
	{
		public string? Id { get; set; }

		public string? Entity { get; set; }

		public long Amount { get; set; }

		public string? Currency { get; set; }

		public string? Receipt { get; set; }

		public string? Status { get; set; }
	}

	private readonly PublisherDbContext _dbContext;

	private readonly IHttpClientFactory _httpClientFactory;

	private readonly ILogger<BillingService> _logger;

	private readonly string _stripeSecretKey;

	private readonly string _stripeWebhookSecret;

	private readonly string _razorpayKeyId;

	private readonly string _razorpayKeySecret;

	private readonly string _razorpayWebhookSecret;

	public BillingService(PublisherDbContext dbContext, IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<BillingService> _logger)
	{
		_dbContext = dbContext;
		_httpClientFactory = httpClientFactory;
		this._logger = _logger;
		_stripeSecretKey = configuration["Stripe:SecretKey"] ?? "sk_test_mock";
		_stripeWebhookSecret = configuration["Stripe:WebhookSecret"] ?? "whsec_mock";
		_razorpayKeyId = configuration["Razorpay:KeyId"] ?? "rzp_test_mock";
		_razorpayKeySecret = configuration["Razorpay:KeySecret"] ?? "mock_secret";
		_razorpayWebhookSecret = configuration["Razorpay:WebhookSecret"] ?? "mock_webhook_secret";
		StripeConfiguration.ApiKey = _stripeSecretKey;
	}

	public async Task<IEnumerable<SubscriptionPlanResponse>> GetSubscriptionPlansAsync(CancellationToken cancellationToken)
	{
		return (await _dbContext.SubscriptionPlans.OrderBy((SubscriptionPlan sp) => sp.Tier).ToListAsync(cancellationToken)).Select((SubscriptionPlan sp) => new SubscriptionPlanResponse(sp.Id, sp.Tier, sp.Name, sp.MonthlyPrice, sp.YearlyPrice, sp.MonthlyVideoLimit));
	}

	public async Task<CheckoutResponse> CreateCheckoutSessionAsync(Guid userId, CreateCheckoutRequest request, CancellationToken cancellationToken)
	{
		if (await _dbContext.Users.FindAsync(new object[1] { userId }, cancellationToken) == null)
		{
			throw new ArgumentException("User not found.", "userId");
		}
		SubscriptionPlan plan = await _dbContext.SubscriptionPlans.FindAsync(new object[1] { request.PlanId }, cancellationToken);
		if (plan == null)
		{
			throw new ArgumentException("Subscription plan not found.", "request");
		}
		decimal price = ((request.BillingCycle.ToLower() == "yearly") ? plan.YearlyPrice : plan.MonthlyPrice);
		if (request.Provider.Equals("Stripe", StringComparison.OrdinalIgnoreCase))
		{
			SessionCreateOptions options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string>(1) { "card" },
				LineItems = new List<SessionLineItemOptions>(1)
				{
					new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(price * 100m),
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = plan.Name + " Plan (" + request.BillingCycle + ")",
								Description = $"Access to Lychee Publisher with {plan.MonthlyVideoLimit} videos/month limit."
							}
						},
						Quantity = 1L
					}
				},
				Mode = "payment",
				SuccessUrl = request.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}",
				CancelUrl = request.CancelUrl,
				Metadata = new Dictionary<string, string>
				{
					{
						"userId",
						userId.ToString()
					},
					{
						"planId",
						plan.Id.ToString()
					},
					{
						"tier",
						plan.Tier.ToString()
					},
					{ "billingCycle", request.BillingCycle }
				}
			};
			SessionService service = new SessionService();
			Session session = await service.CreateAsync(options, null, cancellationToken);
			return new CheckoutResponse(session.Id, session.Url, null);
		}
		if (request.Provider.Equals("Razorpay", StringComparison.OrdinalIgnoreCase))
		{
			HttpClient client = _httpClientFactory.CreateClient();
			string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(_razorpayKeyId + ":" + _razorpayKeySecret));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
			long amountInPaise = (long)(price * 100m);
			var body = new
			{
				amount = amountInPaise,
				currency = "INR",
				receipt = userId.ToString(),
				notes = new Dictionary<string, string>
				{
					{
						"userId",
						userId.ToString()
					},
					{
						"planId",
						plan.Id.ToString()
					},
					{
						"tier",
						plan.Tier.ToString()
					},
					{ "billingCycle", request.BillingCycle }
				}
			};
			string jsonBody = JsonSerializer.Serialize(body);
			StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync("https://api.razorpay.com/v1/orders", content, cancellationToken);
			if (!response.IsSuccessStatusCode)
			{
				string errorText = await response.Content.ReadAsStringAsync(cancellationToken);
				_logger.LogError("Razorpay Order creation failed: {Error}", errorText);
				throw new InvalidOperationException("Failed to initiate payment with Razorpay.");
			}
			RazorpayOrderResponse responseData = await response.Content.ReadFromJsonAsync<RazorpayOrderResponse>(cancellationToken);
			if (responseData == null || string.IsNullOrEmpty(responseData.Id))
			{
				throw new InvalidOperationException("Razorpay response is invalid.");
			}
			return new CheckoutResponse(responseData.Id, "", responseData.Id);
		}
		throw new ArgumentException("Unsupported payment provider.", "request");
	}

	public async Task<bool> ProcessStripeWebhookAsync(string jsonPayload, string signatureHeader, CancellationToken cancellationToken)
	{
		try
		{
			Event stripeEvent = EventUtility.ConstructEvent(jsonPayload, signatureHeader, _stripeWebhookSecret, 300L);
			_logger.LogInformation("Processing Stripe webhook event: {EventType}", stripeEvent.Type);
			if (stripeEvent.Type == "checkout.session.completed")
			{
				Session session = stripeEvent.Data.Object as Session;
				if (session?.Metadata != null && session.Metadata.TryGetValue("userId", out var userIdStr) && session.Metadata.TryGetValue("tier", out var tierStr))
				{
					Guid userId = Guid.Parse(userIdStr);
					int tier = int.Parse(tierStr);
					User user = await _dbContext.Users.FindAsync(new object[1] { userId }, cancellationToken);
					if (user != null)
					{
						user.SubscriptionTier = (SubscriptionTier)tier;
						Payment payment = new Payment
						{
							UserId = userId,
							Provider = "Stripe",
							ProviderPaymentId = session.Id,
							Amount = (decimal)session.AmountTotal.GetValueOrDefault() / 100m,
							Currency = (session.Currency ?? "usd"),
							Status = "Completed"
						};
						_dbContext.Payments.Add(payment);
						await _dbContext.SaveChangesAsync(cancellationToken);
						_logger.LogInformation("Successfully upgraded user {UserId} to tier {Tier} via Stripe", userId, tier);
						return true;
					}
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			_logger.LogError(ex2, "Error processing Stripe webhook.");
			return false;
		}
	}

	public async Task<bool> ProcessRazorpayWebhookAsync(string jsonPayload, string signatureHeader, CancellationToken cancellationToken)
	{
		try
		{
			if (!VerifyRazorpaySignature(jsonPayload, signatureHeader, _razorpayWebhookSecret))
			{
				_logger.LogWarning("Invalid Razorpay webhook signature");
				return false;
			}
			using JsonDocument doc = JsonDocument.Parse(jsonPayload);
			JsonElement root = doc.RootElement;
			string eventName = root.GetProperty("event").GetString();
			_logger.LogInformation("Processing Razorpay webhook event: {EventName}", eventName);
			if ((eventName == "order.paid" || eventName == "payment.captured") && root.TryGetProperty("payload", out var payloadObj))
			{
				JsonElement orderObj = default(JsonElement);
				JsonElement paymentObj = default(JsonElement);
				bool hasOrder = payloadObj.TryGetProperty("order", out orderObj);
				payloadObj.TryGetProperty("payment", out paymentObj);
				JsonElement targetObj = (hasOrder ? orderObj.GetProperty("entity") : paymentObj.GetProperty("entity"));
				if (targetObj.TryGetProperty("notes", out var notesObj))
				{
					JsonElement u;
					string userIdStr = (notesObj.TryGetProperty("userId", out u) ? u.GetString() : null);
					JsonElement t;
					string tierStr = (notesObj.TryGetProperty("tier", out t) ? t.GetString() : null);
					if (Guid.TryParse(userIdStr, out var userId) && int.TryParse(tierStr, out var tier))
					{
						User user = await _dbContext.Users.FindAsync(new object[1] { userId }, cancellationToken);
						if (user != null)
						{
							user.SubscriptionTier = (SubscriptionTier)tier;
							decimal amount = default(decimal);
							if (targetObj.TryGetProperty("amount", out var amountProp))
							{
								amount = amountProp.GetDecimal() / 100m;
							}
							JsonElement currProp;
							string currency = (targetObj.TryGetProperty("currency", out currProp) ? currProp.GetString() : "INR");
							string paymentId = targetObj.GetProperty("id").GetString() ?? Guid.NewGuid().ToString();
							Payment payment = new Payment
							{
								UserId = userId,
								Provider = "Razorpay",
								ProviderPaymentId = paymentId,
								Amount = amount,
								Currency = (currency ?? "INR"),
								Status = "Completed"
							};
							_dbContext.Payments.Add(payment);
							await _dbContext.SaveChangesAsync(cancellationToken);
							_logger.LogInformation("Successfully upgraded user {UserId} to tier {Tier} via Razorpay", userId, tier);
							return true;
						}
					}
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			_logger.LogError(ex2, "Error processing Razorpay webhook.");
			return false;
		}
	}

	public async Task<IEnumerable<PaymentResponse>> GetUserPaymentHistoryAsync(Guid userId, CancellationToken cancellationToken)
	{
		List<Payment> list = await _dbContext.Payments.Where((Payment p) => p.UserId == userId).ToListAsync(cancellationToken);
		return list.OrderByDescending((Payment p) => p.CreatedAtUtc).Select((Payment p) => new PaymentResponse(p.Id, p.Provider, p.ProviderPaymentId, p.Amount, p.Currency, p.Status, p.CreatedAtUtc));
	}

	public async Task<UserUsageResponse> GetUserUsageAsync(Guid userId, CancellationToken cancellationToken)
	{
		User user = await _dbContext.Users.FindAsync(new object[1] { userId }, cancellationToken);
		if (user == null)
		{
			throw new ArgumentException("User not found.", "userId");
		}
		SubscriptionPlan currentPlan = (await _dbContext.SubscriptionPlans.ToListAsync(cancellationToken)).FirstOrDefault((SubscriptionPlan p) => p.Tier == (int)user.SubscriptionTier) ?? new SubscriptionPlan
		{
			Name = "Free",
			Tier = 1,
			MonthlyVideoLimit = 5
		};
		DateTimeOffset firstDayOfMonth = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);
		List<Video> userVideos = await _dbContext.Videos.Where((Video v) => v.UserId == userId).ToListAsync(cancellationToken);
		int videosCount = userVideos.Count((Video v) => v.CreatedAtUtc >= firstDayOfMonth);
		return new UserUsageResponse(LimitExceeded: videosCount >= currentPlan.MonthlyVideoLimit, CurrentPlanName: currentPlan.Name, CurrentTier: currentPlan.Tier, VideosUsedThisMonth: videosCount, MonthlyVideoLimit: currentPlan.MonthlyVideoLimit);
	}

	public async Task<byte[]> GenerateInvoicePdfAsync(Guid userId, Guid paymentId, CancellationToken cancellationToken)
	{
		Payment payment = await _dbContext.Payments.Include((Payment p) => p.User).SingleOrDefaultAsync((Payment p) => p.Id == paymentId && p.UserId == userId, cancellationToken);
		if (payment == null)
		{
			throw new ArgumentException("Payment record not found.");
		}
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("=================================================================");
		sb.AppendLine("                       Lychee PUBLISHER                        ");
		sb.AppendLine("                        OFFICIAL RECEIPT                         ");
		sb.AppendLine("=================================================================");
		sb.AppendLine();
		StringBuilder stringBuilder = sb;
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(22, 1, stringBuilder);
		handler.AppendLiteral("Receipt Number  : REC-");
		handler.AppendFormatted(payment.Id.ToString().Substring(0, 8).ToUpper());
		stringBuilder2.AppendLine(ref handler);
		stringBuilder = sb;
		StringBuilder stringBuilder3 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(18, 1, stringBuilder);
		handler.AppendLiteral("Receipt Date    : ");
		handler.AppendFormatted(payment.CreatedAtUtc.ToString("F"));
		stringBuilder3.AppendLine(ref handler);
		stringBuilder = sb;
		StringBuilder stringBuilder4 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(21, 2, stringBuilder);
		handler.AppendLiteral("Billing To      : ");
		handler.AppendFormatted(payment.User?.DisplayName ?? "User");
		handler.AppendLiteral(" (");
		handler.AppendFormatted(payment.User?.Email);
		handler.AppendLiteral(")");
		stringBuilder4.AppendLine(ref handler);
		stringBuilder = sb;
		StringBuilder stringBuilder5 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(18, 1, stringBuilder);
		handler.AppendLiteral("Payment Status  : ");
		handler.AppendFormatted(payment.Status.ToUpper());
		stringBuilder5.AppendLine(ref handler);
		stringBuilder = sb;
		StringBuilder stringBuilder6 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(18, 1, stringBuilder);
		handler.AppendLiteral("Payment Provider: ");
		handler.AppendFormatted(payment.Provider);
		stringBuilder6.AppendLine(ref handler);
		stringBuilder = sb;
		StringBuilder stringBuilder7 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(18, 1, stringBuilder);
		handler.AppendLiteral("Provider Ref ID : ");
		handler.AppendFormatted(payment.ProviderPaymentId);
		stringBuilder7.AppendLine(ref handler);
		sb.AppendLine();
		sb.AppendLine("-----------------------------------------------------------------");
		sb.AppendLine("Description                                    Quantity    Amount");
		sb.AppendLine("-----------------------------------------------------------------");
		stringBuilder = sb;
		StringBuilder stringBuilder8 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(59, 2, stringBuilder);
		handler.AppendLiteral("Lychee Subscription - Plan Renewal                1       ");
		handler.AppendFormatted(payment.Amount.ToString("F2"));
		handler.AppendLiteral(" ");
		handler.AppendFormatted(payment.Currency.ToUpper());
		stringBuilder8.AppendLine(ref handler);
		sb.AppendLine("-----------------------------------------------------------------");
		stringBuilder = sb;
		StringBuilder stringBuilder9 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(53, 2, stringBuilder);
		handler.AppendLiteral("TOTAL PAID                                          ");
		handler.AppendFormatted(payment.Amount.ToString("F2"));
		handler.AppendLiteral(" ");
		handler.AppendFormatted(payment.Currency.ToUpper());
		stringBuilder9.AppendLine(ref handler);
		sb.AppendLine("=================================================================");
		sb.AppendLine();
		sb.AppendLine("Thank you for using Lychee Publisher!");
		sb.AppendLine("If you have any questions, reach out to billing@Lychee.com.");
		return Encoding.UTF8.GetBytes(sb.ToString());
	}

	private static bool VerifyRazorpaySignature(string payload, string signature, string secret)
	{
		if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secret))
		{
			return false;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(secret);
		byte[] bytes2 = Encoding.UTF8.GetBytes(payload);
		using HMACSHA256 hMACSHA = new HMACSHA256(bytes);
		byte[] array = hMACSHA.ComputeHash(bytes2);
		string text = BitConverter.ToString(array).Replace("-", "").ToLower();
		return text.Equals(signature, StringComparison.OrdinalIgnoreCase);
	}
}
