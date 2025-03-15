using Microsoft.Extensions.Options;
using Redplcs.PshenkaFeed.Domain;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Redplcs.PshenkaFeed.Infrastructure.Telegram;

public sealed class TelegramContentSender(ITelegramBotClient api, IOptions<TelegramOptions> options) : IContentSender
{
	private readonly ChatId _chatId = options.Value.ChatId;
	private readonly int _messageThreadId = options.Value.TopicId;

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
		await api.SendPhoto(_chatId, url, messageThreadId: _messageThreadId, cancellationToken: cancellationToken);
	}

	private async Task SendVideoAsync(string url, CancellationToken cancellationToken)
	{
		await api.SendVideo(_chatId, url, messageThreadId: _messageThreadId, cancellationToken: cancellationToken);
	}
}
