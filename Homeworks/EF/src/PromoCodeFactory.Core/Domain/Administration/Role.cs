using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Role
        : BaseEntity
    {
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}