using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeUpdateDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AppliedPromocodesCount { get; set; }
    }
}
