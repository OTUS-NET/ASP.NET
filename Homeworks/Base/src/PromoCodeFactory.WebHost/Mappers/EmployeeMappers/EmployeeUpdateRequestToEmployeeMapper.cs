using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.WebHost.Mappers.EmployeeMappers;

public class EmployeeUpdateRequestToEmployeeMapper : IMapper<EmployeeUpdateRequest, Employee>
{
    /// <inheritdoc />
    public Employee Map(EmployeeUpdateRequest source)
    {
        return new Employee
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email
        };
    }
}