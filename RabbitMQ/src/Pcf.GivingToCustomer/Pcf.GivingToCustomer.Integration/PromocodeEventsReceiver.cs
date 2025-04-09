using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Pcf.GivingToCustomer.Core.Services;
using Pcf.Shared.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration;

public class PromocodeEventsReceiver(
    ConnectionFactory factory,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly ConnectionFactory _factory = factory;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(3000, stoppingToken); 

        Console.WriteLine("PromocodeEventsReceiver is starting.");

        using var connection = await _factory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync("promocode-events",
            ExchangeType.Topic,
            durable: true,
            cancellationToken: stoppingToken);

        var queue = await channel.QueueDeclareAsync("giving-to-customer-promocode-on-creating",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(queue.QueueName,
            "promocode-events",
            "created.*",
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            Console.WriteLine("ReceivedAsync in a consumer");

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            PromoCodeAppliedEvent? dto = null;

            try
            {
                dto = JsonSerializer.Deserialize<PromoCodeAppliedEvent>(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при десериализации: {ex.Message}");
            }

            if (dto != null)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<PromocodeService>();

                    await service.GivePromoCodesToCustomersWithPreference(dto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке сообщения {message}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Deserialized DTO оказался null.");
            }

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            Console.WriteLine("Acknowledged message: " + ea.DeliveryTag);
        };

        var consumerTag = await channel.BasicConsumeAsync(queue.QueueName, autoAck: false, consumer, cancellationToken: stoppingToken);
        Console.WriteLine($"Consumer is set up and waiting for messages. Consumer tag: {consumerTag}");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
