using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Dtos;

public record EmployeeDto(string FirstName, string LastName, string Email, List<Guid> Roles)
{
    public Employee MapToEntity()
    {
        var entity = new Employee
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email
        };

        return entity;
    }
}
