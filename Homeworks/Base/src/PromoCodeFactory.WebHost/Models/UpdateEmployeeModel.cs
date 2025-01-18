using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class UpdateEmployeeModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AppliedPromocodesCount { get; set; }

        public List<RoleItemRequest> Roles { get; set; }
    }
}
