using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace PromoCodeFactory.WebHost.Models.Request
{
    public class CreateOrEditCustomerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<Guid> PreferenceIds { get; set; }
    }
}
