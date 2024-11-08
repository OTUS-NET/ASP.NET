using System;

namespace PromoCodeFactory.WebHost.Models;

public sealed class EmployeeRequest : AddEmployeeRequest
{
    public Guid Id { get; set; }
}
