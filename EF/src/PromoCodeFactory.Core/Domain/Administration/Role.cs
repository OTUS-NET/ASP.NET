using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Role
        : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Relations
        public ICollection<Employee> Employees { get; set; }
    }
}