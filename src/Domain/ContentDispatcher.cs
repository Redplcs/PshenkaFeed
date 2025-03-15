namespace Redplcs.PshenkaFeed.Domain;

public class ContentDispatcher(IContentReceiver receiver, IContentSender sender)
{
	public async Task WaitForIncomingAndSendAsync(CancellationToken cancellationToken)
	{
		var content = await receiver.WaitForIncomingAsync(cancellationToken);

		await sender.SendAsync(content, cancellationToken);
	}
}
