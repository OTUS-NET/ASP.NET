using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.Shared.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration;

public sealed class RabbitService(ConnectionFactory factory)
{
    private readonly ConnectionFactory factory = factory;

    public async Task SendPromocodeCreatedEvent(PromoCode promocode)
    {
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync("promocode-events",
            ExchangeType.Topic,
            durable: true);

        var dto = new PromoCodeAppliedEvent
        {
            Code = promocode.Code,
            ServiceInfo = promocode.ServiceInfo,
            BeginDate = promocode.BeginDate,
            EndDate = promocode.EndDate,
            PartnerId = promocode.PartnerId,
            PreferenceId = promocode.PreferenceId,
            PartnerManagerId = promocode.PartnerManagerId
        };
        var message = JsonSerializer.Serialize(dto);
        var body = Encoding.UTF8.GetBytes(message);
        var managerKeyPart = promocode.PartnerManagerId.HasValue ? "withmanager" : "withoutmanager";
        var routingKey = "created." + managerKeyPart;
        await channel.BasicPublishAsync("promocode-events", routingKey, body);
    }
}
