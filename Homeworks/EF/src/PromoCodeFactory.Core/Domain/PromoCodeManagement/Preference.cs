using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference : BaseEntity
    {
        public string Name { get; set; }
        public virtual List<Customer> Customers { get; set; }
    }
}