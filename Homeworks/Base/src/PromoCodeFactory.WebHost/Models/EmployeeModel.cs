using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeModel
    {
        public Guid? Id { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public required string Email { get; set; }

        public required List<RoleModel> Roles { get; set; }

        public required int AppliedPromocodesCount { get; set; }
    }
}
