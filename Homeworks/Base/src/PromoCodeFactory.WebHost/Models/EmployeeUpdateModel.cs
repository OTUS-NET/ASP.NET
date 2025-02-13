using System;

namespace PromoCodeFactory.WebHost.Models;

public class EmployeeUpdateModel
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Email { get; set; }

    public int AppliedPromocodesCount { get; set; }
}
