using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }

        public List<Role> Roles { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}