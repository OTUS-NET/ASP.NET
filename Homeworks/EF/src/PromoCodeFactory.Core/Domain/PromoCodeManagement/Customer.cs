using PromoCodeFactory.Core.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
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
        
        [StringLength(512)]
        public string? Address { get; set; }

        public ICollection<Preference> Preferences { get; set; } = new List<Preference>();
        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
    }
}