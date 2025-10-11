using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions
{
    public interface IRabbitMqProducer
    {
        Task ProduceAsync<T>(T message, string routingKey) where T : class;
    }
}
