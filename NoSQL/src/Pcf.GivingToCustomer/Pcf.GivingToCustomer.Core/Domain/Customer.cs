using System.Collections.Generic;

namespace Pcf.GivingToCustomer.Core.Domain
{
    public class Customer
        :BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public virtual ICollection<CustomerPreference> Preferences { get; set; }
        
        public virtual ICollection<PromoCodeCustomer> PromoCodes { get; set; }
    }
}