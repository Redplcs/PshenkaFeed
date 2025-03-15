using Microsoft.Extensions.Options;
using Redplcs.PshenkaFeed.Domain;
using Telegram.Bot;

namespace Redplcs.PshenkaFeed.Infrastructure.Telegram;

public sealed class TelegramContentSender(ITelegramBotClient api, IOptions<TelegramOptions> options) : IContentSender
{
	public Task SendAsync(Content content, CancellationToken cancellationToken)
	{
		return content.Type switch
		{
			ContentType.Photo => SendPhotoAsync(content.Url, cancellationToken),
			ContentType.Video => SendVideoAsync(content.Url, cancellationToken),
			_ => throw new ArgumentException("The content has invalid type.", nameof(content))
		};
	}

	private async Task SendPhotoAsync(string url, CancellationToken cancellationToken)
	{
		await api.SendPhoto(options.Value.ChatId, url, cancellationToken: cancellationToken);
	}

	private async Task SendVideoAsync(string url, CancellationToken cancellationToken)
	{
		await api.SendVideo(options.Value.ChatId, url, cancellationToken: cancellationToken);
	}
}
