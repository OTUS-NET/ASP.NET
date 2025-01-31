using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        // Создаем и настраиваем хост
        // CreateDefaultBuilder нужен для создания хоста с необходимостью ручной настройки основных параметров IHost (DI,IConfiguration,ILogger)
        var host = Host.CreateDefaultBuilder(args)  
            .ConfigureAppConfiguration((context, config) =>
            {
                // Добавляем конфигурацию из appsettings.json
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {

                // Регистрируем настройки как сервис
                services.Configure<DemoSettings>(context.Configuration.GetSection("DemoSettings"));

                // Регистрация сервисов
                services.AddSingleton<IEmailService, EmailService>();

                 // Регистрируем сервис для работы с жизненным циклом приложения
                services.AddSingleton<AppLifecycleService>();

                // Можно зарегистрировать другие службы или зависимости
            })
            .Build(); // Создаем хост


        // Используем сервис в хосте
        host.Services.GetService<IEmailService>().SendReports();

        // Получаем настройки и выводим их значения
        var mySettings = host.Services.GetRequiredService<IOptions<DemoSettings>>().Value;
        Console.WriteLine($"Message: {mySettings.Message}");
        Console.WriteLine($"Year: {mySettings.Year}");

        // Инициализация сервиса работы с жизненным циклом приложения
        host.Services.GetService<AppLifecycleService>();

        // Запускаем хост
        await host.RunAsync();
    }
}



