using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Сервис для управления жизненным циклом приложения.
/// </summary>
public class AppLifecycleService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger<AppLifecycleService> _logger;
    private readonly IEmailService emailService;

    public AppLifecycleService(
        IHostApplicationLifetime appLifetime,
        ILogger<AppLifecycleService> logger,
        IEmailService emailService)
    {
        _appLifetime = appLifetime;
        _logger = logger;
        this.emailService = emailService;

        // Подписываемся на события жизненного цикла
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);
    }

    private void OnStarted()
    {
        _logger.LogInformation("Приложение запустилось.");
        // Здесь можно выполнить задачи, которые должны быть выполнены при запуске приложения
        // Например, инициализация ресурсов, загрузка данных и т.д.
        emailService.NotifyAdmin("Приложение запустилось.");
    }

    private void OnStopping()
    {
        _logger.LogInformation("Приложение останавливается...");
        // Здесь можно выполнить задачи, которые должны быть выполнены перед остановкой приложения
        // Например, освобождение ресурсов, сохранение состояния и т.д.
    }

    private void OnStopped()
    {
        _logger.LogInformation("Приложение остановлено.");
        // Здесь можно выполнить задачи, которые должны быть выполнены после остановки приложения
        // Например, логирование завершения работы, отправка уведомлений и т.д.
        emailService.NotifyAdmin("Приложение остановлено.");
        Thread.Sleep(3000);
    }
}
