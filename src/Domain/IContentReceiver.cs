namespace Redplcs.PshenkaFeed.Domain;

public interface IContentReceiver
{
	Task<Content> WaitForIncomingAsync(CancellationToken cancellationToken);
}
