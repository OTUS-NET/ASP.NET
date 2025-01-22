using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models;

public class CreateOrUpdateEmployeeRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Email { get; set; }

    public List<Guid> RoleIds { get; set; }
}