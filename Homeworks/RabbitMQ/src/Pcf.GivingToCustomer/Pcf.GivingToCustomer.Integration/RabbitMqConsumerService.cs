using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.Core.Models;
using Pcf.SharedLibrary.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration
{
    public class RabbitMqConsumerService(
        RabbitMqConnectionService connectionService,
        IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private IChannel _channel;
        private readonly RabbitMqConnectionService _connectionService = connectionService;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private const string QueueName = "promocode.issued.giving";
        private const string ExchangeName = "pcf.events";
        private const string RoutingKey = "promocode.issued";
        private const int MaxRetries = 10,
                          DelayMs = 2000;

        private async Task<IConnection> GetConnectionWithRetryAsync(CancellationToken ct)
        {
            // Ждём, пока подключение станет доступным
            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    return _connectionService.GetConnection();
                }
                catch (InvalidOperationException)
                {
                    if (i == MaxRetries - 1)
                        throw new InvalidOperationException("RabbitMQ connection not ready after retries");
                    await Task.Delay(DelayMs, ct);
                }
            }
            throw new InvalidOperationException("RabbitMQ connection not ready");
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            var connection = await GetConnectionWithRetryAsync(ct);
            _channel = await connection.CreateChannelAsync(cancellationToken: ct);
            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true, cancellationToken: ct);
            await _channel.QueueDeclareAsync(QueueName, durable: true, cancellationToken: ct);
            await _channel.QueueBindAsync(QueueName, ExchangeName, RoutingKey, cancellationToken: ct);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, e) =>
            {
                var eventToConsume = JsonSerializer.Deserialize<PromoCodeIssuedEvent>(e.Body.ToArray());

                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPromoCodeService>();
                var request = new GivePromoCodeRequest()
                {
                    PartnerId = eventToConsume.PartnerId,
                    BeginDate = eventToConsume.BeginDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EndDate = eventToConsume.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    PreferenceId = eventToConsume.PreferenceId,
                    PromoCode = eventToConsume.Code,
                    ServiceInfo = eventToConsume.ServiceInfo
                };

                await service.GivePromoCodesToCustomersWithPreferenceAsync(request);

                await _channel.BasicAckAsync(e.DeliveryTag, multiple: false);
            };
            await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, cancellationToken: ct);
            await Task.CompletedTask.WaitAsync(ct);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _channel = null;
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
