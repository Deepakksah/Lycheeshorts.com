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

public static class ScheduleEndpoints
{
	public static IEndpointRouteBuilder MapScheduleEndpoints(this IEndpointRouteBuilder app)
	{
		RouteGroupBuilder endpoints = app.MapGroup("/api/v1/schedules").RequireAuthorization().WithTags("Schedules");
		endpoints.MapPost("/", (Func<CreateScheduleRequest, ClaimsPrincipal, ISchedulingService, CancellationToken, Task<IResult>>)async delegate(CreateScheduleRequest request, ClaimsPrincipal user, ISchedulingService schedulingService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			if (userId == Guid.Empty)
			{
				return Results.Unauthorized();
			}
			try
			{
				ScheduleResponse response = await schedulingService.CreateScheduleAsync(userId, request, cancellationToken);
				return Results.Created($"/api/v1/schedules/{response.Id}", response);
			}
			catch (InvalidOperationException ex)
			{
				return Results.ValidationProblem(new Dictionary<string, string[]> { ["schedule"] = new string[1] { ex.Message } });
			}
		}).WithName("CreateSchedule");
		endpoints.MapGet("/", (Func<ClaimsPrincipal, ISchedulingService, CancellationToken, Task<IResult>>)async delegate(ClaimsPrincipal user, ISchedulingService schedulingService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			return (userId == Guid.Empty) ? Results.Unauthorized() : Results.Ok(await schedulingService.GetUserSchedulesAsync(userId, cancellationToken));
		}).WithName("GetSchedules");
		endpoints.MapPatch("/{scheduleId:guid}", (Func<Guid, UpdateScheduleRequest, ClaimsPrincipal, ISchedulingService, CancellationToken, Task<IResult>>)async delegate(Guid scheduleId, UpdateScheduleRequest request, ClaimsPrincipal user, ISchedulingService schedulingService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			if (userId == Guid.Empty)
			{
				return Results.Unauthorized();
			}
			try
			{
				ScheduleResponse response = await schedulingService.UpdateScheduleAsync(scheduleId, userId, request, cancellationToken);
				return ((object)response == null) ? Results.NotFound() : Results.Ok(response);
			}
			catch (InvalidOperationException ex)
			{
				return Results.ValidationProblem(new Dictionary<string, string[]> { ["schedule"] = new string[1] { ex.Message } });
			}
		}).WithName("UpdateSchedule");
		endpoints.MapDelete("/{scheduleId:guid}", (Func<Guid, ClaimsPrincipal, ISchedulingService, CancellationToken, Task<IResult>>)async delegate(Guid scheduleId, ClaimsPrincipal user, ISchedulingService schedulingService, CancellationToken cancellationToken)
		{
			Guid userId = GetUserId(user);
			if (userId == Guid.Empty)
			{
				return Results.Unauthorized();
			}
			try
			{
				return (await schedulingService.DeleteScheduleAsync(scheduleId, userId, cancellationToken)) ? Results.NoContent() : Results.NotFound();
			}
			catch (InvalidOperationException ex)
			{
				return Results.ValidationProblem(new Dictionary<string, string[]> { ["schedule"] = new string[1] { ex.Message } });
			}
		}).WithName("DeleteSchedule");
		return app;
	}

	private static Guid GetUserId(ClaimsPrincipal user)
	{
		string input = user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
		Guid result;
		return Guid.TryParse(input, out result) ? result : Guid.Empty;
	}
}
