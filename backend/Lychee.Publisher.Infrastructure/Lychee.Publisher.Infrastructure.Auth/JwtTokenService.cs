using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Lychee.Publisher.Application.Abstractions;
using Lychee.Publisher.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lychee.Publisher.Infrastructure.Auth;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
	private readonly JwtOptions _options = options.Value;

	public AccessTokenResult CreateAccessToken(User user)
	{
		DateTimeOffset expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(_options.AccessTokenMinutes);
		SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
		SigningCredentials signingCredentials = new SigningCredentials(key, "HS256");
		List<Claim> claims = new List<Claim>
		{
			new Claim("sub", user.Id.ToString()),
			new Claim("email", user.Email),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Id.ToString()),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", user.Email),
			new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", user.Role),
			new Claim("display_name", user.DisplayName ?? user.Email)
		};
		JwtSecurityToken token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, DateTime.UtcNow, expiresAtUtc.UtcDateTime, signingCredentials);
		return new AccessTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
	}

	public RefreshTokenResult CreateRefreshToken()
	{
		byte[] bytes = RandomNumberGenerator.GetBytes(64);
		string text = Convert.ToBase64String(bytes);
		return new RefreshTokenResult(text, HashRefreshToken(text), DateTimeOffset.UtcNow.AddDays(_options.RefreshTokenDays));
	}

	public string HashRefreshToken(string refreshToken)
	{
		byte[] inArray = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
		return Convert.ToHexString(inArray);
	}
}
