using System;

namespace Lychee.Publisher.Application.Validation;

public sealed class ValidationException : InvalidOperationException
{
	public ValidationException(string message)
		: base(message)
	{
	}
}
