using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lychee.Publisher.Api.Endpoints;

public static class PaymentEndpoints
{
	public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
	{
		RouteGroupBuilder endpoints = app.MapGroup("/api/v1/payments").WithTags("Payments");
		endpoints.MapGet("/plans", (Func<IBillingService, CancellationToken, Task<IResult>>)(async (IBillingService billingService, CancellationToken cancellationToken) => Results.Ok(await billingService.GetSubscriptionPlansAsync(cancellationToken)))).WithName("GetPlans");
		endpoints.MapPost("/checkout", (Func<CreateCheckoutRequest, ClaimsPrincipal, IBillingService, CancellationToken, Task<IResult>>)async delegate(CreateCheckoutRequest request, ClaimsPrincipal user, IBillingService billingService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			if (userId == Guid.Empty)
			{
				return Results.Unauthorized();
			}
			try
			{
				return Results.Ok(await billingService.CreateCheckoutSessionAsync(userId, request, cancellationToken));
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(new
				{
					error = ex.Message
				});
			}
			catch (InvalidOperationException ex2)
			{
				return Results.BadRequest(new
				{
					error = ex2.Message
				});
			}
			catch (Exception ex3)
			{
				return Results.Problem(ex3.Message);
			}
		}).RequireAuthorization().WithName("CreateCheckoutSession");
		endpoints.MapGet("/history", (Func<ClaimsPrincipal, IBillingService, CancellationToken, Task<IResult>>)async delegate(ClaimsPrincipal user, IBillingService billingService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			return (userId == Guid.Empty) ? Results.Unauthorized() : Results.Ok(await billingService.GetUserPaymentHistoryAsync(userId, cancellationToken));
		}).RequireAuthorization().WithName("GetPaymentHistory");
		endpoints.MapGet("/usage", (Func<ClaimsPrincipal, IBillingService, CancellationToken, Task<IResult>>)async delegate(ClaimsPrincipal user, IBillingService billingService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			return (userId == Guid.Empty) ? Results.Unauthorized() : Results.Ok(await billingService.GetUserUsageAsync(userId, cancellationToken));
		}).RequireAuthorization().WithName("GetUserUsage");
		endpoints.MapPost("/webhook/stripe", (Func<HttpRequest, IBillingService, CancellationToken, Task<IResult>>)async delegate(HttpRequest request, IBillingService billingService, CancellationToken cancellationToken)
		{
			string signature = request.Headers["Stripe-Signature"].ToString();
			if (string.IsNullOrEmpty(signature))
			{
				return Results.BadRequest("Missing Stripe-Signature header.");
			}
			using StreamReader reader = new StreamReader(request.Body);
			return (await billingService.ProcessStripeWebhookAsync(await reader.ReadToEndAsync(cancellationToken), signature, cancellationToken)) ? Results.Ok() : Results.BadRequest("Webhook processing failed.");
		}).WithName("StripeWebhook");
		endpoints.MapPost("/webhook/razorpay", (Func<HttpRequest, IBillingService, CancellationToken, Task<IResult>>)async delegate(HttpRequest request, IBillingService billingService, CancellationToken cancellationToken)
		{
			string signature = request.Headers["X-Razorpay-Signature"].ToString();
			if (string.IsNullOrEmpty(signature))
			{
				return Results.BadRequest("Missing X-Razorpay-Signature header.");
			}
			using StreamReader reader = new StreamReader(request.Body);
			return (await billingService.ProcessRazorpayWebhookAsync(await reader.ReadToEndAsync(cancellationToken), signature, cancellationToken)) ? Results.Ok() : Results.BadRequest("Webhook processing failed.");
		}).WithName("RazorpayWebhook");
		endpoints.MapGet("/history/{paymentId:guid}/invoice", (Func<Guid, ClaimsPrincipal, IBillingService, CancellationToken, Task<IResult>>)async delegate(Guid paymentId, ClaimsPrincipal user, IBillingService billingService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			if (userId == Guid.Empty)
			{
				return Results.Unauthorized();
			}
			try
			{
				return Results.File(await billingService.GenerateInvoicePdfAsync(userId, paymentId, cancellationToken), "text/plain", $"receipt_{paymentId}.txt");
			}
			catch (Exception ex)
			{
				return Results.BadRequest(new
				{
					error = ex.Message
				});
			}
		}).RequireAuthorization().WithName("GetInvoiceReceipt");
		return app;
	}

	private static Guid GetUserId(ClaimsPrincipal user)
	{
		string input = user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
		Guid result;
		return Guid.TryParse(input, out result) ? result : Guid.Empty;
	}
}
