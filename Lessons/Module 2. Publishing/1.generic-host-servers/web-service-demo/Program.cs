using Microsoft.OpenApi.Models;

// WebApplication - это преднастроенная версия IHost для веб-приложений ASP.NET
var builder = WebApplication.CreateBuilder(args);

// Вывод на консоль текущего окружения, которое было указано в конфигурации приложения.
Console.WriteLine($"ASPNETCORE_ENVIRONMENT:   {builder.Configuration["ASPNETCORE_ENVIRONMENT"]}");

// Настройка Kestrel - HTTP-инфраструктуры, которая будет использована в качестве сервера.
builder.WebHost.ConfigureKestrel((context, options) =>
{
    // Получение настроек Kestrel из конфигурации приложения.
    var kestrelSettings = context.Configuration.GetSection("Kestrel");
    
    // Конфигурирование Kestrel по полученным настройкам.
    options.Configure(kestrelSettings);
});

// Добавление в сервисы приложения экземпляров, которые предоставляют функции API и Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Настройка заголовка Swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Generic Host - Minimal API Demo", Version = "v1" });
});

// Создание веб-приложения по конфигурации, которая была построена ранее.
var app = builder.Build();

// Включение Swagger и UI при старте приложения.
app.UseSwagger();
app.UseSwaggerUI();

// Создание списка с сообщениями для демонстрации функции API.
List<string> messages = new List<string>() { "Привет, Generic Host!" };

// Добавление роутинга веб-приложения, которая будет перенаправлять всех пользователей на страницу Swagger при доступе по корневому URL.
app.MapGet("/", () => Results.Redirect("/swagger"));

// Создание роутинга для получения списка сообщений.
app.MapGet("api/", () => messages);

// Добавление роутинга, который будет добавлять новое сообщение в список при получении запроса POST.
app.MapPost("api/", (string message) =>
{
    // Добавление полученного сообщения в список.
    messages.Add(message);
    
    // Возвращение списка с новым сообщением.
    return messages;
});

// Добавление роутинга, который будет удалять сообщение по индексу при получении запроса DELETE.
app.MapDelete("api/{id}", (int id) =>
{
    // Удаление сообщения из списка по заданному индексу.
    messages.RemoveAt(id);
    
    // Возвращение списка после удаления сообщения.
    return messages;
});

// Добавление роутинга, который будет изменять сообщение по индексу при получении запроса PUT.
app.MapPut("api/{id}", (int id, string message) =>
{
    // Установка значения в списке с заданным индексом новым сообщением.
    messages[id] = message;
    
    // Возвращение списка после изменения сообщения.
    return messages;
});

// Запуск веб-приложения.
app.Run();