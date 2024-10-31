using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    public interface IGivingPromoCodeToCustomerGateway
    {
        Task GivePromoCodeToCustomer(PromoCode promoCode);
    }
}