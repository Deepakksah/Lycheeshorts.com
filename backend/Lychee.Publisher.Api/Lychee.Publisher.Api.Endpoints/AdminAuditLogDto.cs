using System;

namespace Lychee.Publisher.Api.Endpoints;

public record AdminAuditLogDto(Guid Id, Guid? UserId, string Action, string? EntityName, string? EntityId, string? MetadataJson, DateTimeOffset CreatedAtUtc);
