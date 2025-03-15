using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redplcs.PshenkaFeed.Domain;
using Redplcs.PshenkaFeed.Infrastructure.Telegram;

var host = Host.CreateApplicationBuilder(args);

host.Services.Configure<TelegramOptions>(host.Configuration.GetSection("Telegram"));
host.Services.AddSingleton<IContentSender, TelegramContentSender>();

host.Build().Run();
