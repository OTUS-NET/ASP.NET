using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models;

public class AddEmployeeRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public IEnumerable<RoleItemRequest> Roles { get; set; }

    public int AppliedPromocodesCount { get; set; }
}
