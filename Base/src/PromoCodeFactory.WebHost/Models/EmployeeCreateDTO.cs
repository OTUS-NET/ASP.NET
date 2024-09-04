using PromoCodeFactory.Core.Domain.Administration;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeCreateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AppliedPromocodesCount { get; set; }
    }
}
