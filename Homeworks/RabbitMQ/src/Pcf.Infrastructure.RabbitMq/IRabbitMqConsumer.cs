using RabbitMQ.Client;

namespace Pcf.Infrastructure.RabbitMq;

public interface IRabbitMqConsumer<TMessage>
{
    public Task Register(IChannel channel, string exchangeName, string queueName, string routingKey);
}