using System;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers.EmployeeMappers;

public class EmployeeCreationRequestToEmployeeMapper : IMapper<EmployeeCreationRequest, Core.Domain.Administration.Employee>
{
    /// <inheritdoc />
    public Core.Domain.Administration.Employee Map(EmployeeCreationRequest source)
    {
        return new Core.Domain.Administration.Employee
        {
            Id = Guid.NewGuid(),
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email,
            Roles = [],
            AppliedPromocodesCount = 0
        };
    }
}