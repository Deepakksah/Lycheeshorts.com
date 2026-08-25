using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lychee.Publisher.Infrastructure.Persistence;

public sealed class PublisherDbContextFactory : IDesignTimeDbContextFactory<PublisherDbContext>
{
	public PublisherDbContext CreateDbContext(string[] args)
	{
		DbContextOptionsBuilder<PublisherDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<PublisherDbContext>();
		dbContextOptionsBuilder.UseSqlServer("Server=localhost;Database=LycheePublisher;Trusted_Connection=True;TrustServerCertificate=True");
		return new PublisherDbContext(dbContextOptionsBuilder.Options);
	}
}
