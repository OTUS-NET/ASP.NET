using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; }

        public Guid PartnerId { get; set; }

        public string PromoCode { get; set; }

        public Guid PreferenceId { get; set; }
    }
}