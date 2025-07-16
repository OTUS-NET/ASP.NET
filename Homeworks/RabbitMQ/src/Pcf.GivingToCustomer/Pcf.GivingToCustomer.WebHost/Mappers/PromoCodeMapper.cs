using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Models;
using Pcf.GivingToCustomer.WebHost.Services.Promocodes;

namespace Pcf.GivingToCustomer.WebHost.Mappers
{
    public class PromoCodeMapper
    {
        public static PromoCode MapFromModel(GivePromoCodeRequest request, Preference preference, IEnumerable<Customer> customers)
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

            promocode.Customers = new List<PromoCodeCustomer>();

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
        
        public static PromoCode MapFromModel(GivePromoCodeToCustomerDto dto, Preference preference, IEnumerable<Customer> customers)
        {
            var promocode = new PromoCode();
            promocode.Id = dto.PromoCodeId;

            promocode.PartnerId = dto.PartnerId;
            promocode.Code = dto.PromoCode;
            promocode.ServiceInfo = dto.ServiceInfo;

            promocode.BeginDate = DateTime.Parse(dto.BeginDate);
            promocode.EndDate = DateTime.Parse(dto.EndDate);

            promocode.Preference = preference;
            promocode.PreferenceId = preference.Id;

            promocode.Customers = new List<PromoCodeCustomer>();

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
