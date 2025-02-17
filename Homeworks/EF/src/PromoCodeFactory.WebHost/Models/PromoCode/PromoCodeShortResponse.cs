using System;

namespace PromoCodeFactory.WebHost.Models.PromoCode
{
    public class PromoCodeShortResponse
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string ServiceInfo { get; set; }

        public string BeginDate { get; set; }

        public string EndDate { get; set; }

        public string PartnerName { get; set; }
    }
}