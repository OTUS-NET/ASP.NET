using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Models;

public class EmployeeAddRequest
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }

    public string Email { get; set; }
}