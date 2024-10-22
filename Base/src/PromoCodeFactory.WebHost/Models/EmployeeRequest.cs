using System.Collections.Generic;
namespace PromoCodeFactory.WebHost.Models
{
	public class EmployeeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<RoleItemRequest> Roles { get; set; }
        public int AppliedPromocodesCount { get; set; }
    }

	public class RoleItemRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}