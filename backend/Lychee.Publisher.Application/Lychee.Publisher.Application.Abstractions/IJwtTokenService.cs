using Lychee.Publisher.Domain.Entities;

namespace Lychee.Publisher.Application.Abstractions;

public interface IJwtTokenService
{
	AccessTokenResult CreateAccessToken(User user);

	RefreshTokenResult CreateRefreshToken();

	string HashRefreshToken(string refreshToken);
}
