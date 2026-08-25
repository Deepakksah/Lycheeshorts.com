using System.Threading;
using System.Threading.Tasks;

namespace Lychee.Publisher.Application.Abstractions;

public interface IAuthService
{
	Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ipAddress, CancellationToken cancellationToken);

	Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken);

	Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken cancellationToken);

	Task VerifyEmailAsync(string token, string? ipAddress, CancellationToken cancellationToken);

	Task ForgotPasswordAsync(string email, string? ipAddress, CancellationToken cancellationToken);

	Task ResetPasswordAsync(string token, string newPassword, string? ipAddress, CancellationToken cancellationToken);
}
