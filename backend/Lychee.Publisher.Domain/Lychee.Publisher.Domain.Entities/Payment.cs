using System;
using Lychee.Publisher.Domain.Common;

namespace Lychee.Publisher.Domain.Entities;

public sealed class Payment : AuditableEntity
{
	public Guid UserId { get; set; }

	public User? User { get; set; }

	public required string Provider { get; set; }

	public required string ProviderPaymentId { get; set; }

	public decimal Amount { get; set; }

	public required string Currency { get; set; }

	public required string Status { get; set; }
}
