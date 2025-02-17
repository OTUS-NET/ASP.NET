using System;
using PromoCodeFactory.WebHost.Models.Role;

namespace PromoCodeFactory.WebHost.Models.Employee
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public RoleItemResponse Role { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}