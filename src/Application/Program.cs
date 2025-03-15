using Microsoft.Extensions.Hosting;
using Redplcs.PshenkaFeed.Application.Configurations;

// Let portainer to set environment variables before services will throw exceptions
Thread.Sleep(5000);

var host = Host.CreateApplicationBuilder(args);

host.Services
	.AddVkontakte(host.Configuration)
	.AddTelegram(host.Configuration)
	.AddDispatcherWorker();

host.Build().Run();
