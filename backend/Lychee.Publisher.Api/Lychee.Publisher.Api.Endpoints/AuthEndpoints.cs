using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Lychee.Publisher.Api.Endpoints;

public static class AuthEndpoints
{
	public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
	{
		RouteGroupBuilder endpoints = app.MapGroup("/api/v1/auth").WithTags("Auth").RequireRateLimiting("auth");
		endpoints.MapPost("/register", (Func<RegisterRequest, HttpContext, IAuthService, CancellationToken, Task<IResult>>)async delegate(RegisterRequest request, HttpContext httpContext, IAuthService authService, CancellationToken cancellationToken)
		{
			try
			{
				AuthResponse response = await authService.RegisterAsync(request, httpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
				return Results.Created($"/api/v1/users/{response.UserId}", response);
			}
			catch (InvalidOperationException ex)
			{
				InvalidOperationException ex2 = ex;
				return Results.ValidationProblem(new Dictionary<string, string[]> { ["auth"] = new string[1] { ex2.Message } });
			}
		}).AllowAnonymous().WithName("Register");
		endpoints.MapPost("/login", (Func<LoginRequest, HttpContext, IAuthService, CancellationToken, Task<IResult>>)async delegate(LoginRequest request, HttpContext httpContext, IAuthService authService, CancellationToken cancellationToken)
		{
			try
			{
				return Results.Ok(await authService.LoginAsync(request, httpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken));
			}
			catch (UnauthorizedAccessException ex)
			{
				UnauthorizedAccessException ex2 = ex;
				return Results.Problem(ex2.Message, null, 401, "Unauthorized");
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message, null, 500, "Internal Error");
			}
		}).AllowAnonymous().WithName("Login");
		endpoints.MapPost("/refresh", (Func<RefreshTokenRequest, HttpContext, IAuthService, CancellationToken, Task<IResult>>)async delegate(RefreshTokenRequest request, HttpContext httpContext, IAuthService authService, CancellationToken cancellationToken)
		{
			try
			{
				return Results.Ok(await authService.RefreshAsync(request, httpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken));
			}
			catch (UnauthorizedAccessException ex)
			{
				UnauthorizedAccessException ex2 = ex;
				return Results.Problem(ex2.Message, null, 401, "Unauthorized");
			}
		}).AllowAnonymous().WithName("RefreshToken");
		endpoints.MapPost("/verify-email", (Func<string, HttpContext, IAuthService, CancellationToken, Task<IResult>>)async delegate(string token, HttpContext httpContext, IAuthService authService, CancellationToken cancellationToken)
		{
			try
			{
				await authService.VerifyEmailAsync(token, httpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
				return Results.Ok(new
				{
					message = "Email verified successfully."
				});
			}
			catch (InvalidOperationException ex)
			{
				InvalidOperationException ex2 = ex;
				return Results.BadRequest(new
				{
					error = ex2.Message
				});
			}
		}).AllowAnonymous().WithName("VerifyEmail");
		endpoints.MapPost("/forgot-password", (Func<ForgotPasswordRequest, HttpContext, IAuthService, CancellationToken, Task<IResult>>)async delegate(ForgotPasswordRequest request, HttpContext httpContext, IAuthService authService, CancellationToken cancellationToken)
		{
			await authService.ForgotPasswordAsync(request.Email, httpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
			return Results.Ok(new
			{
				message = "If the email exists, a password reset token has been generated."
			});
		}).AllowAnonymous().WithName("ForgotPassword");
		endpoints.MapPost("/reset-password", (Func<ResetPasswordRequest, HttpContext, IAuthService, CancellationToken, Task<IResult>>)async delegate(ResetPasswordRequest request, HttpContext httpContext, IAuthService authService, CancellationToken cancellationToken)
		{
			try
			{
				await authService.ResetPasswordAsync(request.Token, request.NewPassword, httpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
				return Results.Ok(new
				{
					message = "Password reset successfully."
				});
			}
			catch (InvalidOperationException ex)
			{
				InvalidOperationException ex2 = ex;
				return Results.BadRequest(new
				{
					error = ex2.Message
				});
			}
		}).AllowAnonymous().WithName("ResetPassword");
		endpoints.MapGet("/me", (Func<ClaimsPrincipal, IResult>)((ClaimsPrincipal user) => Results.Ok(new
		{
			userId = user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"),
			email = user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"),
			displayName = user.FindFirstValue("display_name"),
			role = user.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
		}))).RequireAuthorization().WithName("CurrentUser");
		return app;
	}
}
