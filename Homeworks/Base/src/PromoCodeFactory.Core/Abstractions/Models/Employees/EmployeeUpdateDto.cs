using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Abstractions.Models.Employees
{
    public class EmployeeUpdateDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public List<Guid> RoleIds { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}
