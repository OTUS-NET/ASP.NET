using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.Integration;
using Pcf.GivingToCustomer.IntegrationTests.Data;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    /// <summary>
    /// Тестовая фабрика веб-приложения.
    /// Переопределяет MongoDB-настройки приложения, чтобы API-тесты работали с отдельной тестовой базой.
    /// </summary>
    /// <typeparam name="TStartup">Startup-класс тестируемого приложения.</typeparam>
    public class TestWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        /// <summary>
        /// Настраивает тестовый web host перед запуском API-тестов.
        /// Удаляет production Mongo-настройки из DI, добавляет тестовые настройки и заполняет тестовую базу начальными данными.
        /// </summary>
        /// <param name="builder">Конструктор web host, предоставленный Microsoft.AspNetCore.Mvc.Testing.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });

            builder.ConfigureServices(services =>
            {
                // Удаляем Mongo-настройки основного приложения, чтобы тесты не писали в рабочую базу.
                var settingsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(MongoDbSettings));
                if (settingsDescriptor != null)
                    services.Remove(settingsDescriptor);

                var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(MongoContext));
                if (contextDescriptor != null)
                    services.Remove(contextDescriptor);

                services.AddScoped<INotificationGateway, NotificationGateway>();

                services.AddSingleton(new MongoDbSettings
                {
                    ConnectionString = "mongodb://127.0.0.1:27017/?serverSelectionTimeoutMS=3000",
                    DatabaseName = "promocode_factory_giving_to_customer_api_tests"
                });
                services.AddSingleton<MongoContext>();

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<MongoContext>();

                new EfTestDbInitializer(dbContext).InitializeDb();
            });
        }
    }
}
