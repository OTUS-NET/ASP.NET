namespace AspApiBasic.Extensions;

using Microsoft.Extensions.DependencyInjection;

public static class ApiCorsPolicies
{
    public const string AllowAllOrigins = "AllowAllOrigins";
    public const string AllowSpecificOrigin = "AllowSpecificOrigin";
    public const string AllowMultipleOrigins = "AllowMultipleOrigins";
    public const string AllowSpecificRoute = "AllowSpecificRoute";

}

public static class CorsExtensions
{
    // Метод расширения для настройки CORS
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            // Настройка политик CORS для API (можно добавить несколько политик)
            options.AddPolicy(ApiCorsPolicies.AllowAllOrigins, policy =>
            {
                policy.AllowAnyOrigin() // Разрешить запросы с любого домена
                    .AllowAnyMethod() // Разрешить любые HTTP-методы
                    .AllowAnyHeader(); // Разрешить любые заголовки
            });

            options.AddPolicy(ApiCorsPolicies.AllowSpecificOrigin, policy =>
            {
                policy.WithOrigins("https://localhost:5272") // Разрешить только запросы с этого домена
                    .WithMethods("GET", "POST") // Разрешить только GET и POST
                    .WithHeaders("Content-Type", "Authorization"); // Разрешить только указанные заголовки
            });

            options.AddPolicy(ApiCorsPolicies.AllowMultipleOrigins, policy =>
            {
                policy.WithOrigins("https://example.com","http://example.com", "http://localhost:5272") // Разрешить несколько доменов
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            options.AddPolicy(ApiCorsPolicies.AllowSpecificRoute, policy =>
            {
                policy.WithOrigins("http://localhost:5683")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); // Разрешить использование credentials
            });
        });

        return services;
    }
}







/*//Подключение CORS
 // Добавляем сервисы CORS с помощью метода расширения
builder.Services.AddCorsPolicy();

var app = builder.Build();

// Используем middleware CORS
app.UseCors(ApiCorsPolicies.AllowAllOrigins); // Применяем политику CORS по умолчанию для всех запросов
  
 */