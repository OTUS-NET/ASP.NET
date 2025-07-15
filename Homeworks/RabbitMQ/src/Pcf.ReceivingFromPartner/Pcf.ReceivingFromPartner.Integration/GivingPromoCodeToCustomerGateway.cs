using System.Threading.Tasks;
using Pcf.Infrastructure.RabbitMq;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.Dto;
using RabbitMQ.Client;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerGateway : 
        RabbitMqProducer<GivePromoCodeToCustomerDto>,
        IGivingPromoCodeToCustomerGateway
    {
        public GivingPromoCodeToCustomerGateway(
            string exchangeType,
            string exchangeName, 
            string routingKey,
            IChannel channel):
            base(exchangeType, exchangeName,routingKey, channel) { }

        public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        {
            var message = new GivePromoCodeToCustomerDto()
            {
                PartnerId = promoCode.Partner.Id,
                BeginDate = promoCode.BeginDate.ToShortDateString(),
                EndDate = promoCode.EndDate.ToShortDateString(),
                PreferenceId = promoCode.PreferenceId,
                PromoCode = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerManagerId = promoCode.PartnerManagerId
            };

            await Publish(message);
        }
    }
}