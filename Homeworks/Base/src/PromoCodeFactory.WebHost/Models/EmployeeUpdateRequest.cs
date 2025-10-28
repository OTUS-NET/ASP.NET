using System;
using System.Collections.Generic;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.WebHost.Models;

public class EmployeeUpdateRequest
{
    public Guid Id { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }

    public string? Email { get; set; }
}