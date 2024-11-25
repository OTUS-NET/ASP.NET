using System;
using System.Collections.Generic;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Mappers;

public class PromoCodeMapper
{
    public static PromoCode MapFromModel(GivePromoCodeRequest request, Preference preference,
        IEnumerable<Customer> customers)
    {

        var promocode = new PromoCode();
        promocode.Id = request.PromoCodeId;

        promocode.PartnerId = request.PartnerId;
        promocode.Code = request.PromoCode;
        promocode.ServiceInfo = request.ServiceInfo;

        promocode.BeginDate = DateTime.Parse(request.BeginDate);
        promocode.EndDate = DateTime.Parse(request.EndDate);

        promocode.Preference = preference;
        promocode.PreferenceId = preference.Id;

        return promocode;
    }
}
