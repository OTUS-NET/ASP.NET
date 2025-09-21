using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference : BaseEntity
    {
        public string Name { get; set; }
        [JsonIgnore]
        public ICollection<CustomerPreference> CustomerPreferences { get; set; }
        [JsonIgnore]
        public ICollection<PromoCode> PromoCodes { get; set; }

    }
}