using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.IntegrationTests.Data;
using Pcf.GivingToCustomer.IntegrationTests.Fakes;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    public class TestWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        private const string TestDatabaseName = "PromoCodeFactoryGivingToCustomerApiTest";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
            
                var descriptor = services.FindDescriptor(typeof(IMongoDatabase));
                if (descriptor != null)
                services.Remove(descriptor);

                services.AddSingleton<IMongoDatabase>(sp =>
                    sp.GetRequiredService<IMongoClient>().GetDatabase(TestDatabaseName));
                
                services.AddScoped<INotificationGateway, FakeNotificationGateway>();

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                var logger = scope.ServiceProvider
                    .GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();
                
                try
                {
                    new MongoDbTestInitializer(db).InitializeDb();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Проблема во время заполнения тестовой базы. Ошибка: {Message}", ex.Message);
                }
            });
        }
    }

    internal static class ServiceCollectionExtensions
    {
        public static ServiceDescriptor FindDescriptor(this IServiceCollection services, Type serviceType)
        {
            foreach (var descriptor in services)
                if (descriptor.ServiceType == serviceType)
                    return descriptor;
            return null;
        }
    }
}
