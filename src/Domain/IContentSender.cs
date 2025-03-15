namespace Redplcs.PshenkaFeed.Domain;

public interface IContentSender
{
	Task SendAsync(Content content, CancellationToken cancellationToken);
}
