using System;
using System.Threading.Tasks;
using Pcf.Infrastructure.RabbitMq;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Dto;
using RabbitMQ.Client;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class AdministrationGateway : RabbitMqProducer<AdministrationDto>, IAdministrationGateway
    {
        public AdministrationGateway(string exchangeType, string exchangeName, string routingKey, IChannel channel) :
            base(exchangeType, exchangeName,routingKey, channel) { }

        public async Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId)
        {
            var message = new AdministrationDto()
            {
                PartnerId = partnerManagerId
            };

            await Publish(message);
        }
    }
}