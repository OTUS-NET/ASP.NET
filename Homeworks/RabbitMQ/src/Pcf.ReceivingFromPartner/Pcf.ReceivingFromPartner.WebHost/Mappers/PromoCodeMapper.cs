using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Models;
using System;

namespace Pcf.ReceivingFromPartner.WebHost.Mappers
{
    public class PromoCodeMapper
    {
        public static PromoCode MapFromModel(ReceivingPromoCodeRequest request, Preference preference, Partner partner)
        {

            var promocode = new PromoCode
            {
                PartnerId = partner.Id,
                Partner = partner,
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,

                BeginDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),

                Preference = preference,
                PreferenceId = preference.Id,

                PartnerManagerId = request.PartnerManagerId
            };

            return promocode;
        }
    }
}
