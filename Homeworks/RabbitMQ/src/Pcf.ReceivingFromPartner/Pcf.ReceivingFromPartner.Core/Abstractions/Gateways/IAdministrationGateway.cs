using System;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    public interface IAdministrationGateway
    {
        Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId);
    }
}