using System.Collections.Generic;
using System;
using PromoCodeFactory.WebHost.Models.Roles;

namespace PromoCodeFactory.WebHost.Models.Employees
{
    public class EmployeeDto
    {
        public virtual Guid Id { get; set; }

        public virtual string Email { get; set; }

        public virtual int AppliedPromocodesCount { get; set; }
    }
}
