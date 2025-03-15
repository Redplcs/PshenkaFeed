namespace Redplcs.PshenkaFeed.Infrastructure.Telegram;

public sealed class TelegramOptions
{
	public const string TokenKey = "Telegram:Token";

	public long ChatId { get; set; }
	public int TopicId { get; set; }
}
