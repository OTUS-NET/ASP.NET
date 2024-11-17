using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        /// <summary>
        /// Previous
        /// </summary>
        //public virtual ICollection<CustomerPreference> Preferences { get; set; }
        public virtual IEnumerable<CustomerPreference> CustomerPreferences { get; set; }
        public virtual IEnumerable<PromoCode> PromoCodes { get; set; }
    }
}