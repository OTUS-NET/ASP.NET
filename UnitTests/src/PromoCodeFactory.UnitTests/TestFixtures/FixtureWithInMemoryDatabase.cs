using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.WebHost;
using System;
using System.Linq;

namespace PromoCodeFactory.UnitTests.TestFixtures;

public sealed class FixtureWithInMemoryDatabase : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    public FixtureWithInMemoryDatabase()
    {
        var configuration = new ConfigurationBuilder().Build();
        var startup = new Startup(configuration);
        var services = new ServiceCollection();
        startup.ConfigureServices(services);

        var realDbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
        if (realDbContext != null)
            services.Remove(realDbContext);

        services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(databaseName: "TestDatabase"));

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
    }
}
