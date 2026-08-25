using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Lychee.Publisher.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lychee.Publisher.Infrastructure.Auth;

public sealed class AuthService(PublisherDbContext dbContext, IPasswordHasher<User> passwordHasher, IJwtTokenService jwtTokenService) : IAuthService
{
	public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ipAddress, CancellationToken cancellationToken)
	{
		string email = NormalizeEmail(request.Email);
		ValidatePassword(request.Password);
		if (await dbContext.Users.AnyAsync((User user2) => user2.Email == email, cancellationToken))
		{
			throw new InvalidOperationException("A user with this email already exists.");
		}
		User user = new User
		{
			Email = email,
			PasswordHash = string.Empty,
			DisplayName = (string.IsNullOrWhiteSpace(request.DisplayName) ? email : request.DisplayName.Trim()),
			Role = "User"
		};
		user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
		string verificationToken = Guid.NewGuid().ToString("N");
		user.EmailVerificationTokens.Add(new EmailVerificationToken
		{
			TokenHash = HashToken(verificationToken),
			ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(24.0),
			CreatedByIp = ipAddress
		});
		RefreshTokenResult refreshToken = jwtTokenService.CreateRefreshToken();
		user.RefreshTokens.Add(new RefreshToken
		{
			TokenHash = refreshToken.TokenHash,
			ExpiresAtUtc = refreshToken.ExpiresAtUtc,
			CreatedByIp = ipAddress
		});
		dbContext.Users.Add(user);
		await dbContext.SaveChangesAsync(cancellationToken);
		Console.WriteLine("[SECURITY] Email verification token generated for " + email + ": " + verificationToken);
		AccessTokenResult accessToken = jwtTokenService.CreateAccessToken(user);
		return CreateResponse(user, accessToken, refreshToken);
	}

	public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken)
	{
		string email = NormalizeEmail(request.Email);
		User user = await dbContext.Users.Include((User candidate) => candidate.RefreshTokens).SingleOrDefaultAsync((User candidate) => candidate.Email.ToLower() == email, cancellationToken);
		if (user == null)
		{
			throw new UnauthorizedAccessException("Invalid email or password.");
		}
		PasswordVerificationResult passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
		if (passwordResult == PasswordVerificationResult.Failed)
		{
			throw new UnauthorizedAccessException("Invalid email or password.");
		}
		if (!user.IsEmailVerified)
		{
			throw new UnauthorizedAccessException("Please verify your email before logging in.");
		}
		user.LastLoginAtUtc = DateTimeOffset.UtcNow;
		if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
		{
			user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
		}
		RefreshTokenResult refreshToken = jwtTokenService.CreateRefreshToken();
		RefreshToken refreshTokenEntity = new RefreshToken
		{
			TokenHash = refreshToken.TokenHash,
			ExpiresAtUtc = refreshToken.ExpiresAtUtc,
			CreatedByIp = ipAddress,
			UserId = user.Id
		};
		dbContext.RefreshTokens.Add(refreshTokenEntity);
		await dbContext.SaveChangesAsync(cancellationToken);
		AccessTokenResult accessToken = jwtTokenService.CreateAccessToken(user);
		return CreateResponse(user, accessToken, refreshToken);
	}

	public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.RefreshToken))
		{
			throw new UnauthorizedAccessException("Refresh token is required.");
		}
		string tokenHash = jwtTokenService.HashRefreshToken(request.RefreshToken);
		RefreshToken existingToken = await dbContext.RefreshTokens.Include((RefreshToken token) => token.User).SingleOrDefaultAsync((RefreshToken token) => token.TokenHash == tokenHash, cancellationToken);
		if (existingToken?.User == null || !existingToken.IsActive(DateTimeOffset.UtcNow))
		{
			throw new UnauthorizedAccessException("Refresh token is invalid or expired.");
		}
		RefreshTokenResult newRefreshToken = jwtTokenService.CreateRefreshToken();
		existingToken.RevokedAtUtc = DateTimeOffset.UtcNow;
		existingToken.RevokedByIp = ipAddress;
		existingToken.ReplacedByTokenHash = newRefreshToken.TokenHash;
		RefreshToken newRefreshTokenEntity = new RefreshToken
		{
			TokenHash = newRefreshToken.TokenHash,
			ExpiresAtUtc = newRefreshToken.ExpiresAtUtc,
			CreatedByIp = ipAddress,
			UserId = existingToken.User.Id
		};
		dbContext.RefreshTokens.Add(newRefreshTokenEntity);
		await dbContext.SaveChangesAsync(cancellationToken);
		return CreateResponse(accessToken: jwtTokenService.CreateAccessToken(existingToken.User), user: existingToken.User, refreshToken: newRefreshToken);
	}

	public async Task VerifyEmailAsync(string token, string? ipAddress, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(token))
		{
			throw new InvalidOperationException("Verification token is required.");
		}
		string tokenHash = HashToken(token);
		EmailVerificationToken dbToken = await dbContext.EmailVerificationTokens.Include((EmailVerificationToken t) => t.User).SingleOrDefaultAsync((EmailVerificationToken t) => t.TokenHash == tokenHash, cancellationToken);
		if (dbToken == null || !dbToken.IsActive(DateTimeOffset.UtcNow))
		{
			throw new InvalidOperationException("Verification token is invalid or expired.");
		}
		dbToken.UsedAtUtc = DateTimeOffset.UtcNow;
		dbToken.User.IsEmailVerified = true;
		await dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task ForgotPasswordAsync(string email, string? ipAddress, CancellationToken cancellationToken)
	{
		string normalizedEmail = NormalizeEmail(email);
		User user = await dbContext.Users.SingleOrDefaultAsync((User u) => u.Email == normalizedEmail, cancellationToken);
		if (user != null)
		{
			string resetToken = Guid.NewGuid().ToString("N");
			PasswordResetToken resetTokenEntity = new PasswordResetToken
			{
				TokenHash = HashToken(resetToken),
				ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(2.0),
				CreatedByIp = ipAddress,
				UserId = user.Id
			};
			dbContext.PasswordResetTokens.Add(resetTokenEntity);
			await dbContext.SaveChangesAsync(cancellationToken);
			Console.WriteLine("[SECURITY] Password reset token generated for " + email + ": " + resetToken);
		}
	}

	public async Task ResetPasswordAsync(string token, string newPassword, string? ipAddress, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(token))
		{
			throw new InvalidOperationException("Password reset token is required.");
		}
		ValidatePassword(newPassword);
		string tokenHash = HashToken(token);
		PasswordResetToken dbToken = await dbContext.PasswordResetTokens.Include((PasswordResetToken t) => t.User).SingleOrDefaultAsync((PasswordResetToken t) => t.TokenHash == tokenHash, cancellationToken);
		if (dbToken == null || !dbToken.IsActive(DateTimeOffset.UtcNow))
		{
			throw new InvalidOperationException("Password reset token is invalid or expired.");
		}
		dbToken.UsedAtUtc = DateTimeOffset.UtcNow;
		dbToken.User.PasswordHash = passwordHasher.HashPassword(dbToken.User, newPassword);
		await dbContext.SaveChangesAsync(cancellationToken);
	}

	private static string HashToken(string token)
	{
		byte[] inArray = SHA256.HashData(Encoding.UTF8.GetBytes(token));
		return Convert.ToHexString(inArray).ToLowerInvariant();
	}

	private static AuthResponse CreateResponse(User user, AccessTokenResult accessToken, RefreshTokenResult refreshToken)
	{
		return new AuthResponse(user.Id, user.Email, user.DisplayName ?? user.Email, user.Role, accessToken.Token, accessToken.ExpiresAtUtc, refreshToken.Token, refreshToken.ExpiresAtUtc);
	}

	private static string NormalizeEmail(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			throw new InvalidOperationException("Email address or username is required.");
		}
		string text = email.Trim().ToLowerInvariant();
		if (text == "admin")
		{
			return "admin@lychee.com";
		}
		return text;
	}

	private static void ValidatePassword(string password)
	{
		if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
		{
			throw new InvalidOperationException("Password must be at least 8 characters long.");
		}
	}
}
