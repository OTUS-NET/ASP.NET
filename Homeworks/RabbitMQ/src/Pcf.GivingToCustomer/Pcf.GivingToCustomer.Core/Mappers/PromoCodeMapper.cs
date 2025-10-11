using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Pcf.GivingToCustomer.Core.Mappers
{
    public class PromoCodeMapper
    {
        public static PromoCode MapFromModel(GivePromoCodeRequest request, Preference preference, IEnumerable<Customer> customers)
        {
            var promocode = new PromoCode
            {
                Id = request.PromoCodeId,
                PartnerId = request.PartnerId,
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,
                BeginDate = DateTime.SpecifyKind(
                DateTime.ParseExact(request.BeginDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(
                DateTime.ParseExact(request.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTimeKind.Utc),
                Preference = preference,
                PreferenceId = preference.Id,
                Customers = []
            };

            foreach (var item in customers)
            {
                promocode.Customers.Add(new PromoCodeCustomer()
                {

                    CustomerId = item.Id,
                    Customer = item,
                    PromoCodeId = promocode.Id,
                    PromoCode = promocode
                });
            };

            return promocode;
        }
    }
}
