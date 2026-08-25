using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Lychee.Publisher.Application.Abstractions;

public interface IFileStorageService
{
	Task<StoredFileResponse> SaveVideoAsync(Guid userId, string originalFileName, string? contentType, long length, Stream content, CancellationToken cancellationToken);
}
