using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public required string Email { get; set; }

        public required List<Role> Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }

    }
}