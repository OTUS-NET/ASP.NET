using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pcf.SharedLibrary.Settings;
using RabbitMQ.Client;

namespace Pcf.Integration
{
    public abstract class RabbitMqConnectionServiceBase(IOptions<ApplicationSettings> options) : IHostedService, IDisposable
    {
        private readonly IOptions<ApplicationSettings> _options = options;
        private IConnection? _connection;
        private const int MaxRetries = 10,
                          DelayMs = 2000;

        public async Task StartAsync(CancellationToken ct)
        {
            var rmq = _options.Value.RmqSettings;
            var factory = new ConnectionFactory
            {
                HostName = rmq.Host,
                VirtualHost = rmq.VHost,
                UserName = rmq.Login,
                Password = rmq.Password
            };

            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(ct);
                    return;
                }
                catch (Exception) when (i < MaxRetries - 1)
                {
                    await Task.Delay(DelayMs, ct);
                }
            }

            throw new InvalidOperationException("Couldn't connect to RabbitMQ after several attempts");
        }

        public async Task StopAsync(CancellationToken ct)
        {
            if (_connection?.IsOpen == true)
                await _connection.CloseAsync(ct);
            Dispose();
        }

        public IConnection GetConnection()
            => _connection ?? throw new InvalidOperationException("RabbitMQ connection not ready");

        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null;
            GC.SuppressFinalize(this);
        }
    }
}
