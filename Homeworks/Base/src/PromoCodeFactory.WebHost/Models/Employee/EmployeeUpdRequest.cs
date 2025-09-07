using System;

namespace PromoCodeFactory.WebHost.Models.Employee;

public class EmployeeUpdRequest
{
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? Email { get; set; }
    
}