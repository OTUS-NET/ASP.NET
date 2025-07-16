using RabbitMQ.Client;

namespace Pcf.Infrastructure.RabbitMq;

public static class RabbitMqConfiguration
{
    public static async Task<IConnection> GetRabbitConnection(RmqSettings rmqSettings)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = rmqSettings.Host,
            VirtualHost = rmqSettings.VHost,
            UserName = rmqSettings.Login,
            Password = rmqSettings.Password
        };

        return await factory.CreateConnectionAsync();
    }
}