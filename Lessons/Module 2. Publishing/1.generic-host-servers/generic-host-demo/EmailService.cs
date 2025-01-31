public interface IEmailService
{
    Task SendReports();
    void NotifyAdmin(string message);
}

public class EmailService : IEmailService
{
    public void NotifyAdmin(string message)
    {
        Console.WriteLine($"Сообщение администратору: {message}");
    }

    public async Task SendReports()
    {
        Console.WriteLine("Подготовка отчетов");
        await Task.Delay(500);
        Console.WriteLine("Отправка отчетов");
        await Task.Delay(500);
        Console.WriteLine("Отчеты отправлены!");
    }
}