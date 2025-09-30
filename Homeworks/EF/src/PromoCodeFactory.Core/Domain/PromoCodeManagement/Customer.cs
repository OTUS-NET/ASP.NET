using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class CustomerPreference : BaseEntity
    {
        public Guid CustomersId { get; set; }
        public Guid PreferencesId { get; set; }
    }
    
    public class Customer
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

        public ICollection<Preference> Preferences { get; set; } = new List<Preference>();
        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
    }
}