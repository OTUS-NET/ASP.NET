using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost;

public class EmployeeRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public List<RoleRequest> Roles { get; set; }
}
