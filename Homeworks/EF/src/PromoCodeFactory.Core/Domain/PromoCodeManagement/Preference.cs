using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();

        public ICollection<CustomerPreference> Customers { get; set; } = new List<CustomerPreference>();
    }
}