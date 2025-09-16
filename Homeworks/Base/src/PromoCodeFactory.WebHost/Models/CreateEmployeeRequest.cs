using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class CreateEmployeeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public List<Guid> RolesIds { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}