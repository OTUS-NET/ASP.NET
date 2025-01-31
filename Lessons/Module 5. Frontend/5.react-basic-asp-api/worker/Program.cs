using worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(() => Console.WriteLine("Application started!"));

lifetime.ApplicationStopping.Register(() => Console.WriteLine("Application stopping!"));

lifetime.ApplicationStopped.Register(() => Console.WriteLine("Application stopped!"));

host.Run();
