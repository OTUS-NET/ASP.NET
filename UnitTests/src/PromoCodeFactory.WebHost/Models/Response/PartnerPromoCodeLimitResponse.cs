using System;

namespace PromoCodeFactory.WebHost.Models.Response
{
    public class PartnerPromoCodeLimitResponse
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string CreateDate { get; set; }
        public string CancelDate { get; set; }
        public string EndDate { get; set; }
        public int Limit { get; set; }
    }
}
