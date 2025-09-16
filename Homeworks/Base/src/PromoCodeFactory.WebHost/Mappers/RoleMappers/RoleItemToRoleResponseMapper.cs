using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers.RoleMappers;

public class RoleItemToRoleResponseMapper : IMapper<RoleItemResponse, Role>
{
    /// <inheritdoc />
    public Role Map(RoleItemResponse source)
    {
        return new Role
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description
        };
    }
}