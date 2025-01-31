// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {
        services.AddHostedService<MyBackgroundService>();
    })
    .Build();

await host.RunAsync();