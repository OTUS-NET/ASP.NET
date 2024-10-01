using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        public string Name { get; set; }
        
        // Relations
        public ICollection<CustomerPreference> CustomersPreferences { get; set; }
        //public ICollection<Customer> Customers { get; set; }
    }
}