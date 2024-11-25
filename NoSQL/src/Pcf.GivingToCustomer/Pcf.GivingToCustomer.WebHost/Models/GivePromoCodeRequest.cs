using System;

namespace Pcf.GivingToCustomer.WebHost.Models
{
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; }

        public string PartnerId { get; set; }

        public string PromoCodeId { get; set; }
        
        public string PromoCode { get; set; }

        public string PreferenceId { get; set; }

        public string BeginDate { get; set; }

        public string EndDate { get; set; }
    }
}