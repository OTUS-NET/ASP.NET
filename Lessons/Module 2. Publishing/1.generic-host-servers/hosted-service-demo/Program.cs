// Создаем новый экземпляр Host, который будет использован для создания приложения с Hosted Service.
// CreateApplicationBuilder уже включает в себя базовую настройку основных параметров IHost (DI,IConfiguration,ILogger)
var builder = Host.CreateApplicationBuilder(args);

// Добавляем в приложение сервис для выполнения фоновой задачи по расписанию
builder.Services.AddHostedService<ScheduledWorkService>();

// Добавляем в приложение сервис для выполнения фоновой задачи по таймеру
builder.Services.AddHostedService<TimedHostedService>();

// Собираем IHost и запускаем его. 
var host = builder.Build();
host.Run();