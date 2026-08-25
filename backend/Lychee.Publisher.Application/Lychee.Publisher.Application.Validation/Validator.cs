using System;

namespace Lychee.Publisher.Application.Validation;

public static class Validator
{
	public static void RequireNotEmpty(string? value, string fieldName)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ValidationException(fieldName + " is required and cannot be empty.");
		}
	}

	public static void RequireEmail(string? email)
	{
		RequireNotEmpty(email, "Email");
		if (!email.Contains('@') || !email.Contains('.'))
		{
			throw new ValidationException("Email address format is invalid.");
		}
		if (email.Length > 320)
		{
			throw new ValidationException("Email address is too long (max 320 characters).");
		}
	}

	public static void RequirePassword(string? password)
	{
		RequireNotEmpty(password, "Password");
		if (password.Length < 8)
		{
			throw new ValidationException("Password must be at least 8 characters.");
		}
		if (password.Length > 128)
		{
			throw new ValidationException("Password is too long (max 128 characters).");
		}
	}

	public static void RequireMinLength(string? value, string fieldName, int min)
	{
		RequireNotEmpty(value, fieldName);
		if (value.Length < min)
		{
			throw new ValidationException($"{fieldName} must be at least {min} characters.");
		}
	}

	public static void RequireMaxLength(string? value, string fieldName, int max)
	{
		if (value != null && value.Length > max)
		{
			throw new ValidationException($"{fieldName} cannot exceed {max} characters.");
		}
	}

	public static void RequirePositive(decimal value, string fieldName)
	{
		if (value < 0m)
		{
			throw new ValidationException(fieldName + " must be a positive number.");
		}
	}

	public static void RequireFutureDate(DateTimeOffset date, string fieldName)
	{
		if (date <= DateTimeOffset.UtcNow)
		{
			throw new ValidationException(fieldName + " must be a future date/time.");
		}
	}

	public static void RequireValidUrl(string? url, string fieldName)
	{
		RequireNotEmpty(url, fieldName);
		if (!Uri.TryCreate(url, UriKind.Absolute, out Uri result) || (result.Scheme != "http" && result.Scheme != "https"))
		{
			throw new ValidationException(fieldName + " must be a valid HTTP/HTTPS URL.");
		}
	}
}
