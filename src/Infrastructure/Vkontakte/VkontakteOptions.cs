namespace Redplcs.PshenkaFeed.Infrastructure.Vkontakte;

public sealed class VkontakteOptions
{
	public const string AccessTokenKey = "Vkontakte:AccessToken";

	public long GroupId { get; set; }
	public ICollection<long> IgnoreUsers { get; set; } = [];
}
