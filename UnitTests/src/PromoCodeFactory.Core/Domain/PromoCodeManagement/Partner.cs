using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Partner
        : BaseEntity
    {
        public string Name { get; set; }
        //Promo codes qnty 
        public int NumberIssuedPromoCodes  { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// Previous
        /// </summary>
        //public virtual ICollection<PartnerPromoCodeLimit> PartnerLimits { get; set; }
        public virtual IList<PartnerPromoCodeLimit> PartnerLimits { get; set; }
    }
}