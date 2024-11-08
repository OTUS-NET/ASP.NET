using System;

namespace PromoCodeFactory.WebHost.Models;

public sealed class EmployeeShortResponse
{
    public Guid Id { get; set; }
    
    public string FullName { get; set; }

    public string Email { get; set; }
}