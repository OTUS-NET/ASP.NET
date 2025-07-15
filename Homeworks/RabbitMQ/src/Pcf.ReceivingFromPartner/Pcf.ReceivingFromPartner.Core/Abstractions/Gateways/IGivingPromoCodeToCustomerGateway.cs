using System.Threading.Tasks;
using Pcf.Infrastructure.RabbitMq;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.Dto;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    public interface IGivingPromoCodeToCustomerGateway : IRabbitMqProducer<GivePromoCodeToCustomerDto>
    {
        Task GivePromoCodeToCustomer(PromoCode promoCode);
    }
}