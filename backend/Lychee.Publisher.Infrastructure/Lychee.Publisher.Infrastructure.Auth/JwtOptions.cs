namespace Lychee.Publisher.Infrastructure.Auth;

public sealed class JwtOptions
{
	public string Issuer { get; set; } = "Lychee.Publisher";

	public string Audience { get; set; } = "Lychee.Publisher.Web";

	public string SigningKey { get; set; } = "REPLACE_WITH_KEY_VAULT_SECRET";

	public int AccessTokenMinutes { get; set; } = 15;

	public int RefreshTokenDays { get; set; } = 30;
}
