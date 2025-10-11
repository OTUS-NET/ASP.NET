using Pcf.ReceivingFromPartner.Core.Abstractions;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class RabbitMqProducer(RabbitMqConnectionService connectionService) : IRabbitMqProducer
    {
        private readonly RabbitMqConnectionService _connectionService = connectionService;
        private const string ExchangeName = "pcf.events";

        public async Task ProduceAsync<T>(T message, string routingKey) where T : class
        {
            var connection = _connectionService.GetConnection();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);
            var messageSerialized = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageSerialized);
            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: routingKey,
                body: body);
        }
    }
}
