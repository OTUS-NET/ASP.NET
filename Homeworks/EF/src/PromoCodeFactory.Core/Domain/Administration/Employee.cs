using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee
        : BaseEntity
    {
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        public string LastName { get; set; }
        
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [StringLength(50)]
        public string Email { get; set; }

        public Role Role { get; set; }

        public int AppliedPromocodesCount { get; set; }
    }
}