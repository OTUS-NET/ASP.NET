using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Dtos.Administation;

public record EmployeeDto
{
    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string Email { get; init; }

    public List<Guid> Roles { get; init; }
}
