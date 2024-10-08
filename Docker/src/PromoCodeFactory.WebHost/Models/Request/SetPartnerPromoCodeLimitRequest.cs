using System;

namespace PromoCodeFactory.WebHost.Models.Request
{
    public class SetPartnerPromoCodeLimitRequest
    {
        public DateTime EndDate { get; set; }
        public int Limit { get; set; }
    }
}
