using Microsoft.Extensions.DependencyInjection;

namespace Lychee.Publisher.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		return services;
	}
}
