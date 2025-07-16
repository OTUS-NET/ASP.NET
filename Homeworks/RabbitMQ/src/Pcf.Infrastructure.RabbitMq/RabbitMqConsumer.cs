using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Pcf.Infrastructure.RabbitMq;

public abstract class RabbitMqConsumer : IRabbitMqConsumer
{
    public async Task Register(IChannel channel, string exchangeName, string queueName, string routingKey)
    {
        await channel.BasicQosAsync(0, 10, false);
        await channel.QueueDeclareAsync(queueName, false, false, false, null);
        await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += async (sender, e) =>
        {
            await OnConsumerOnReceivedAsync(sender, e);
            await channel.BasicAckAsync(e.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(queueName, false, consumer);
    }

    protected abstract Task OnConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs e);
}