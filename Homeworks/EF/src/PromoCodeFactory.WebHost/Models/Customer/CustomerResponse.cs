using System;
using System.Collections.Generic;
using PromoCodeFactory.WebHost.Models.Preference;
using PromoCodeFactory.WebHost.Models.PromoCode;

namespace PromoCodeFactory.WebHost.Models.Customer
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<PromoCodeShortResponse> PromoCodes { get; set; }
        public List<PreferenceShortResponse> Preferences { get; set; }
    }
}