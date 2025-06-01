using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost
{
    public class DbInitializerHostedService : IHostedService
    {
        private readonly IServiceProvider _services;
        public DbInitializerHostedService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = _services.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<IDbInitializer>().InitializeDbAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
