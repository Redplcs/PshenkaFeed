using Microsoft.Extensions.Options;
using Redplcs.PshenkaFeed.Domain;
using System.Diagnostics.CodeAnalysis;
using VkNet;
using VkNet.Abstractions;
using VkNet.Extensions.Polling;
using VkNet.Extensions.Polling.Models.Configuration;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace Redplcs.PshenkaFeed.Infrastructure.Vkontakte;

public sealed class VkontakteContentReceiver(IVkApi api, IOptions<VkontakteOptions> options) : IContentReceiver
{
	private readonly long _groupId = options.Value.GroupId;
	private readonly ICollection<long> _ignoreUsers = options.Value.IgnoreUsers;
	private GroupLongPoll? _longPoll;

	public async Task<Content> WaitForIncomingAsync(CancellationToken cancellationToken)
	{
		EnsureLongPollingStarted(cancellationToken);

		var message = await ReadMessageWithAttachmentAsync(cancellationToken);
		var attachment = message.Attachments.First();

		return attachment.Instance switch
		{
			Photo photo => GetPhotoContent(photo),
			//Video video => GetVideoContent(video), <== video is currently not working
			_ => await WaitForIncomingAsync(cancellationToken)
		};
	}

	private static Content GetPhotoContent(Photo photo)
	{
		var maxSizeUrl = photo.Sizes
			.Select(size => new { PixelCount = size.Width * size.Height, Url = size.Url })
			.OrderByDescending(size => size.PixelCount)
			.First()
			.Url.ToString();

		return new(maxSizeUrl, ContentType.Photo);
	}

	private static Content GetVideoContent(Video video)
	{
		return new(video.UploadUrl.ToString(), ContentType.Video);
	}

	[MemberNotNull(nameof(_longPoll))]
	private void EnsureLongPollingStarted(CancellationToken stoppingToken)
	{
		_longPoll ??= VkApiExtensions.StartGroupLongPollAsync(
			(VkApi)api,
			GroupLongPollConfiguration.Default,
			stoppingToken);
	}

	private async ValueTask<Message> ReadMessageWithAttachmentAsync(CancellationToken cancellationToken)
	{
		var reader = _longPoll!.AsChannelReader();

		while (true)
		{
			var update = await reader.ReadAsync(cancellationToken);

			// skip if received not from specified chat
			if (update.GroupId != (ulong)_groupId)
				continue;

			// skip if not message_new update
			if (update.MessageNew is null)
				continue;

			// skip if user is blacklisted
			if (_ignoreUsers.Contains((long)update.MessageNew.Message.FromId!))
				continue;

			// skip if has 1 attachment
			if (update.MessageNew.Message.Attachments.Count != 1)
				continue;

			return update.MessageNew.Message;
		}
	}
}
