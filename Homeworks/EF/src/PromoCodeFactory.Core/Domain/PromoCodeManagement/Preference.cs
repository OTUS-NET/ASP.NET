using System.Collections.Generic;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<CustomerPreference> CustomerPreferences { get; set; } = new List<CustomerPreference>();

        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
    }
}