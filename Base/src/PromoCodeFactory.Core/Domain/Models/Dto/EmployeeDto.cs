using PromoCodeFactory.Core.Domain.Administration;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Models.Dto
{
    public class EmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AppliedPromocodesCount { get; set; }
        // navigation property
        public List<Role> Roles { get; set; }
    }
}
