namespace Lychee.Publisher.Domain.Enums;

public enum ProcessingStatus
{
	Draft = 1,
	Uploaded,
	Queued,
	Processing,
	Processed,
	Scheduled,
	Publishing,
	Published,
	Failed,
	Cancelled
}
