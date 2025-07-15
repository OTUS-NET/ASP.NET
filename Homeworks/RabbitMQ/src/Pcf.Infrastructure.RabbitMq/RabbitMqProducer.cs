using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Pcf.Infrastructure.RabbitMq;

public class RabbitMqProducer<TMessage> : IRabbitMqProducer<TMessage>
{
    private readonly string _exchangeName;
    private readonly string _routingKey;
    private readonly IChannel _channel;
    
    public RabbitMqProducer(
        string exchangeType,
        string exchangeName,
        string routingKey, 
        IChannel channel)
    {
        _exchangeName = exchangeName;
        _routingKey = routingKey;
        _channel = channel;
        _channel.ExchangeDeclareAsync(_exchangeName, exchangeType);
    }
    
    public async Task Publish(TMessage message)
    {
        var messageSerialized = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageSerialized);
            
        await _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: _routingKey,
            body: body);

        Console.WriteLine($"Message {messageSerialized} is sent into exchange: {_exchangeName}");
    }
}