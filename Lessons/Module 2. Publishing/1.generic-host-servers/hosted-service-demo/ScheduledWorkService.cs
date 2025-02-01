public class ScheduledWorkService : BackgroundService
{
    private readonly ILogger<ScheduledWorkService> _logger;
    private string _lastExecutedDate;
    public ScheduledWorkService(ILogger<ScheduledWorkService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information) && IsScheduledDateTime())
            {
                _logger.LogInformation("DemoBackgroundService выполнена в {time}", DateTimeOffset.Now);
                _lastExecutedDate = DateTime.UtcNow.ToShortDateString();
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

    protected bool IsScheduledDateTime()
    {
        return DateTime.UtcNow.ToShortDateString() != _lastExecutedDate
        && DateTime.UtcNow.Hour == 0 
        && DateTime.UtcNow.Minute == 0;

    }

}
