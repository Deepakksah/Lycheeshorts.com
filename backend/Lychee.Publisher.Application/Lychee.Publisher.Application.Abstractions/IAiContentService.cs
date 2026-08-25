using System.Threading;
using System.Threading.Tasks;

namespace Lychee.Publisher.Application.Abstractions;

public interface IAiContentService
{
	Task<GeneratedShortMetadata> GenerateShortMetadataAsync(string transcript, CancellationToken cancellationToken);
}
