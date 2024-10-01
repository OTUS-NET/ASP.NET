using System;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee
        : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AppliedPromocodesCount { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Relations
        public Guid RoleId { get; set; }
        public Role Role { get; set; }

    }
}