using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Collections;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Models.Response
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<PreferenceResponse> Preferences { get; set; }
        public IEnumerable<PromoCodeShortResponse> PromoCodes { get; set; }
    }
}
