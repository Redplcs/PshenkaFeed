using Microsoft.Extensions.Hosting;
using Redplcs.PshenkaFeed.Application.Configurations;

var host = Host.CreateApplicationBuilder(args);

host.Services
	.AddVkontakte(host.Configuration)
	.AddTelegram(host.Configuration)
	.AddDispatcherWorker();

host.Build().Run();
