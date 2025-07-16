using System;
using System.Threading.Tasks;
using Infrastructure.RabbitMq;
using Pcf.ReceivingFromPartner.Core.Dto;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    public interface IAdministrationGateway : IRabbitMqProducer<AdministrationDto>
    {
        Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId);
    }
}