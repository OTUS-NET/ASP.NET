using System;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Role
        : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Employee> Employees { get; set; }

        public bool IsDeleted { get; set; }
    }
}