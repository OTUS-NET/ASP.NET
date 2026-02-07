using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PromoCode> Promocodes { get; set; }
        public List<CustomerPreference> Customers { get; set; }
    }
}