using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Redplcs.PshenkaFeed.Infrastructure.Vkontakte;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet;
using Redplcs.PshenkaFeed.Domain;
using Redplcs.PshenkaFeed.Infrastructure.Telegram;
using Redplcs.PshenkaFeed.Application.Workers;
using Telegram.Bot;

namespace Redplcs.PshenkaFeed.Application.Configurations;

internal static class DependencyInjection
{
	internal static IServiceCollection AddVkontakte(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<VkontakteOptions>(configuration.GetSection("Vkontakte"));
		services.AddSingleton<IVkApi>(services =>
		{
			var logger = services.GetRequiredService<ILogger<VkApi>>();
			var configuration = services.GetRequiredService<IConfiguration>();

			var api = new VkApi(logger);

			api.Authorize(new ApiAuthParams()
			{
				AccessToken = configuration.GetValue<string>(VkontakteOptions.AccessTokenKey)!,
			});

			return api;
		});
		services.AddSingleton<IContentReceiver, VkontakteContentReceiver>();
		return services;
	}

	internal static IServiceCollection AddTelegram(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));
		services.AddSingleton<ITelegramBotClient>(services =>
		{
			var configuration = services.GetRequiredService<IConfiguration>();

			return new TelegramBotClient(configuration.GetValue<string>(TelegramOptions.TokenKey)!);
		});
		services.AddSingleton<IContentSender, TelegramContentSender>();
		return services;
	}

	internal static IServiceCollection AddDispatcherWorker(this IServiceCollection services)
	{
		return services.AddHostedService<ContentDispatchingWorker>();
	}
}
