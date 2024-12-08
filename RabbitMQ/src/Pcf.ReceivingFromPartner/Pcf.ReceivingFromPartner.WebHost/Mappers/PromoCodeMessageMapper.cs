using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Message;

namespace Pcf.ReceivingFromPartner.WebHost.Mappers
{
    public class PromoCodeMessageMapper
    {
        public static PromoCodeMessage MapInMessage(PromoCode promoCode)
        {

            return new PromoCodeMessage()
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                BeginDate = promoCode.BeginDate,
                EndDate = promoCode.EndDate,
                PartnerId = promoCode.PartnerId,
                PartnerManagerId = promoCode.PartnerManagerId,
                PreferenceId = promoCode.PreferenceId
            };
        }
    }
}
