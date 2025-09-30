using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        [StringLength(50)]
        public string Name { get; set; }
        
        [StringLength(150)]
        public string Value { get; set; }

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}