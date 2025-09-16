using System.Linq;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers.EmployeeMappers;

public class EmployeeToEmployeeResponseMapper(IRoleMappers roleMappers) : IMapper<Core.Domain.Administration.Employee, EmployeeResponse>
{
    /// <inheritdoc />
    public EmployeeResponse Map(Core.Domain.Administration.Employee source)
    {
        return new EmployeeResponse
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email,
            Roles = source.Roles?.Select(roleMappers.RoleToRoleItemResponse.Map).ToList() ?? [],
            AppliedPromocodesCount = source.AppliedPromocodesCount
        };
    }
}