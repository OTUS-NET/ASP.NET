using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public List<PreferenceResponse> Preferences { get; set; }
        public List<PromoCodeShortResponse> PromoCodes { get; set; }
    }
}