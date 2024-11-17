using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference : BaseEntity
    {
        /// <summary>
        /// Previous
        /// </summary>
        //public string Name { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual IEnumerable<PromoCode> PromoCodes { get; set; }
        public virtual IEnumerable<CustomerPreference> CustomerPreferences { get; set; }
    }
}