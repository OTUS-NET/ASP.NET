using System;
using PromoCodeFactory.WebHost.Models.Roles;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PromoCodeFactory.WebHost.Models.Employees
{
    public class EmployeeRequest : EmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual List<Guid> Roles { get; set; }
    }

    public class EmployeeCreateRequest : EmployeeRequest
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override Guid Id { get => base.Id; set => base.Id = value; }
    }
}
