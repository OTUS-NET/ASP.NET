using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Mappers.RoleMappers;

public class RoleToRoleItemResponseMapper : IMapper<Role, RoleItemResponse>
{
    /// <inheritdoc />
    public RoleItemResponse Map(Role source)
    {
        return new RoleItemResponse
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description
        };
    }
}