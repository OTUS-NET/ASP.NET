using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Pcf.GivingToCustomer.WebHost.Mappers
{
    public class PromoCodeMapper
    {
        public static PromoCode MapFromModel(GivePromoCodeRequest request, Guid preferenceId, IEnumerable<Customer> customers) {

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
                PreferenceId = preferenceId,
                Customers = new List<PromoCodeCustomer>()
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
