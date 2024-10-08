using System.Collections.Generic;
using System;

namespace PromoCodeFactory.WebHost.Models.Response
{
    public class PartnerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int NumberIssuedPromoCodes { get; set; }
        public List<PartnerPromoCodeLimitResponse> PartnerLimits { get; set; }
    }
}
