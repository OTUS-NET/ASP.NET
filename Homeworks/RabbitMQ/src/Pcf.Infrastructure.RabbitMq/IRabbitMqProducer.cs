namespace Pcf.Infrastructure.RabbitMq;

public interface IRabbitMqProducer<in TMessage>
{
    public Task Publish(TMessage message);
}