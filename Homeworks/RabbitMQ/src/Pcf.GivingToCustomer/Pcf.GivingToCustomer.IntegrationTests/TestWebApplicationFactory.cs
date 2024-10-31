using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.IntegrationTests.Data;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.Integration;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    public class TestWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<DataContext>));

                services.Remove(descriptor);

                services.AddScoped<INotificationGateway, NotificationGateway>();

                services.AddDbContext<DataContext>(x =>
                {
                    x.UseSqlite("Filename=PromoCodeFactoryDb.sqlite");
                    //x.UseNpgsql(Configuration.GetConnectionString("PromoCodeFactoryDb"));
                    x.UseSnakeCaseNamingConvention();
                    x.UseLazyLoadingProxies();
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<DataContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();

                try
                {
                    new EfTestDbInitializer(dbContext).InitializeDb();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Проблема во время заполнения тестовой базы. " +
                                        "Ошибка: {Message}", ex.Message);
                }
            });
        }
    }
}