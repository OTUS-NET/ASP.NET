using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models.Requests
{
    public class CreateOrEditCustomerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Used to List<Guid>:
        /// public List<Guid> PreferenceIds { get; set; }
        /// </summary>
       
        public IEnumerable<Guid> PreferenceIds { get; set; }
    }
}