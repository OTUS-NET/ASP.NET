using System.Collections.Generic;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.WebHost.Models;

public class EmployeeRequest
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }

    public List<Role> Roles { get; set; }

    public int AppliedPromocodesCount { get; set; }
}
