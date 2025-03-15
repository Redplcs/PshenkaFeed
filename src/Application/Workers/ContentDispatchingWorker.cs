using Microsoft.Extensions.Hosting;
using Redplcs.PshenkaFeed.Domain;

namespace Redplcs.PshenkaFeed.Application.Workers;

internal sealed class ContentDispatchingWorker(IContentReceiver receiver, IContentSender sender) : BackgroundService
{
	private readonly ContentDispatcher _dispatcher = new(receiver, sender);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (stoppingToken.IsCancellationRequested == false)
		{
			await _dispatcher.WaitForIncomingAndSendAsync(stoppingToken);
		}
	}
}
