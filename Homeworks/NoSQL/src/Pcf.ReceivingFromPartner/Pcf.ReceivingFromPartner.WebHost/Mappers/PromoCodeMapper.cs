using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Models;

namespace Pcf.ReceivingFromPartner.WebHost.Mappers
{
    public class PromoCodeMapper
    {
        public static PromoCode MapFromModel(ReceivingPromoCodeRequest request, Guid preferenceId, Partner partner) {

            var promocode = new PromoCode
            {
                PartnerId = partner.Id,
                Partner = partner,
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,

                BeginDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),

                PreferenceId = preferenceId,

                PartnerManagerId = request.PartnerManagerId
            };

            return promocode;
        }
    }
}
