using System;
using System.Collections.Generic;
using PromoCodeFactory.WebHost.Models.Roles;

namespace PromoCodeFactory.WebHost.Models.Employees
{
    public class EmployeeResponse : EmployeeDto
    {
        public virtual string FullName { get; set; }
        public List<RoleDto> Roles { get; set; }
    }
}