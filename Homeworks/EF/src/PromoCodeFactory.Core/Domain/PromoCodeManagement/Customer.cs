using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }

        //TODO: Списки Preferences и Promocodes 
        [JsonIgnore]
        public ICollection<CustomerPreference> CustomerPreferences { get; set; }
        public ICollection<PromoCode> PromoCodes { get; set; }

    }
}