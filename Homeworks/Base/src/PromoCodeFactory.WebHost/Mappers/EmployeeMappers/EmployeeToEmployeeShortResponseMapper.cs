using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers.EmployeeMappers;

public class EmployeeToEmployeeShortResponseMapper : IMapper<Core.Domain.Administration.Employee, EmployeeShortResponse>
{
    /// <inheritdoc />
    public EmployeeShortResponse Map(Core.Domain.Administration.Employee source)
    {
        return new EmployeeShortResponse
        {
            Id = source.Id,
            FullName = $"{source.FirstName} {source.LastName}",
            Email = source.Email
        };
    }
}