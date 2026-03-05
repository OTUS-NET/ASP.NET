using System;

namespace Pcf.GivingToCustomer.Core.Abstractions.Services.Models
{
    public class GivePromoCodeToCustomersWithPreferenceCommand
    {
        public Guid PromoCodeId { get; init; }
        public Guid PartnerId { get; init; }
        public string PromoCode { get; init; }
        public Guid PreferenceId { get; init; }
        public DateTime BeginDate { get; init; }
        public DateTime EndDate { get; init; }
        public string ServiceInfo { get; init; }
    }
}

