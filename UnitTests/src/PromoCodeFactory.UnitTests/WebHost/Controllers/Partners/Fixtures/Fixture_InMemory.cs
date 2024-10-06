using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromoCodeFactory.Compose;
using PromoCodeFactory.WebHost.Helpers;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners.Fixtures
{
    public class Fixture_InMemory : IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }
        public IServiceCollection ServiceCollection { get; set; }

        /// <summary>
        /// Выполняется перед запуском тестов
        /// </summary>
        public Fixture_InMemory()
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.Build();
            ServiceCollection = configuration.GetServiceCollection();
            var serviceProvider = GetServiceProvider();
            ServiceProvider = serviceProvider;
        }

        private IServiceProvider GetServiceProvider()
        {
            var serviceProvider = ServiceCollection
                .ConfigureInMemoryContext()
                .BuildServiceProvider();
            serviceProvider.Seed();
            return serviceProvider;
        }

        public void Dispose()
        {
        }
    }
}
